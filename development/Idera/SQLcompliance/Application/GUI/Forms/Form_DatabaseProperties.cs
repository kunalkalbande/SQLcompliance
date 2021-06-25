using System;
using System.Linq;
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
using Idera.SQLcompliance.Core.Remoting;
using Idera.SQLcompliance.Core.Status;
using Idera.SQLcompliance.Core.Templates.AuditTemplates;
using System.Text;

namespace Idera.SQLcompliance.Application.GUI.Forms
{
    /// <summary>
    /// Summary description for Form_DatabaseProperties.
    /// </summary>
    /// <remarks>
    /// SQLCM-5375 - 6.1.4.1-Greying Logic and Deselection Implementation
    /// </remarks>
    public partial class Form_DatabaseProperties : Form, IDeselectionClient
    {
        public enum Context
        {
            General,
            AuditedActivities,
            DmlFilters,
            TrustedUsers,
            BeforeAfterData,
            SensitiveColumns,
            PrivilegedUser
        };

        #region Properties

        private ServerRecord _server;
        private DatabaseRecord _oldDb;
        public DatabaseRecord _db;
        private List<DatabaseRecord> _alwaysOnDBList = new List<DatabaseRecord>();

        // SQLCM-5375 - 6.1.4.1-Greying Logic and Deselection Implementation
        private DeselectionManager deselectionManager;

        private ICollection _oldTables;
        private bool _isLoaded = false;
        private bool _isDirty = false;
        private string _oldTablesSnapshot = "";
        private string _oldDCTablesSnapshot = "";
        private string _oldSCTablesSnapshot = "";

        private Dictionary<string, DataChangeTableRecord> _oldBATables;
        private Dictionary<string, List<SensitiveColumnTableRecord>> _oldSCTables;

        private SQLDirect _sqlServer = null;
        private ICollection _tableList = null;
        private bool _tryAgentCommunication = false;
        private Dictionary<string, DatabaseObjectRecord> _tableObjects = null;
        private int _compatibilityLevel = -1;
        private bool _beforeAfterAvailable;
        

        //Sqlcm 5.6  start
        private List<String> duplicates;//contains duplicate table list
        //Sqlcm 5.6  End

        #endregion




        public List<DatabaseRecord> AlwaysOnDBList
        {
            get { return _alwaysOnDBList; }
            set
            {
                _alwaysOnDBList = value;
            }
        }


        #region Constructor / Dispose
        //start sqlcm 5.6 - 5364
        ToolTip tip;
        Control _currentToolTipControl;
        //end sqlcm 5.6 - 5364
        public Form_DatabaseProperties(ServerRecord server, DatabaseRecord inDb)
        {
            // Initialize Deselction Manager
            this.deselectionManager = new DeselectionManager(this);

            this.deselectionManager.DatabaseRegulations = RegulationSettings.LoadUserAppliedSettingsDatabase(Globals.Repository.Connection, inDb.SrvId, inDb.DbId);
            this.deselectionManager.LoadSettings();

            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            _sqlServer = new SQLDirect();
            this.Icon = Resources.SQLcompliance_product_ico;
            _lstTrustedUsers.SmallImageList = AppIcons.AppImageList16();
            lstPrivilegedUsers.SmallImageList = AppIcons.AppImageList16();
            lstPreSelectedPrivilegedUsers.SmallImageList = AppIcons.AppImageList16();

            // save database record as original - use for dirty check later
            _oldDb = inDb;
            _server = server;
            //start sqlcm 5.6 - 5364
            tip = new ToolTip();
            _grpAuditActivity.MouseMove += Movecheck;
            grpAuditResults.MouseMove += Movecheck;
            _tabAuditSettings.MouseMove += Movecheck;
            grpAuditUserActivity.MouseMove += Movecheck;
            //end sqlcm 5.6 - 5364
            try
            {
                ServerRecord sr = new ServerRecord();
                sr.Connection = Globals.Repository.Connection;
                sr.Read(inDb.SrvInstance);
                _tryAgentCommunication = sr.IsDeployed && sr.IsRunning;
            }
            catch { }

            _compatibilityLevel = GetCompatibilityLevel();
            // get user tables for save-auditing
            _oldTablesSnapshot = Snapshot.GetDatabaseTables(Globals.Repository.Connection,
                                                            _oldDb.DbId,
                                                            "\t\t");
            _oldDCTablesSnapshot = Snapshot.GetDataChangeTables(Globals.Repository.Connection, _oldDb.DbId, "\t\t");
            _oldSCTablesSnapshot = Snapshot.GetSensitiveColumnTables(Globals.Repository.Connection, _oldDb.DbId, "\t\t");

            // General
            txtServer.Text = inDb.SrvInstance;
            txtName.Text = inDb.Name;
            txtDescription.Text = inDb.Description;

            // status
            txtTimeCreated.Text = GetDateString(inDb.TimeCreated);
            txtTimeLastModified.Text = GetDateString(inDb.TimeLastModified);
            txtTimeEnabledModified.Text = GetDateString(inDb.TimeEnabledModified);
            txtStatus.Text = (inDb.IsEnabled) ? UIConstants.Status_Enabled
                                                             : UIConstants.Status_Disabled;
            // Audit Settings
            _chkAuditDDL.Checked = inDb.AuditDDL;
            _chkAuditSecurity.Checked = inDb.AuditSecurity;
            _chkAuditAdmin.Checked = inDb.AuditAdmin;
            _chkAuditDML.Checked = inDb.AuditDML;
            _chkAuditSELECT.Checked = inDb.AuditSELECT;
            _chkCaptureSQL.Checked = inDb.AuditCaptureSQL;
            _chkCaptureTrans.Checked = inDb.AuditCaptureTrans;
            chkDBCaptureDDL.Checked = inDb.AuditCaptureDDL;

            switch (inDb.AuditAccessCheck)
            {
                case AccessCheckFilter.SuccessOnly:
                    _cbFilterOnAccess.Checked = true;
                    _rbPassed.Checked = true;
                    break;
                case AccessCheckFilter.NoFilter:
                    _cbFilterOnAccess.Checked = false;
                    _rbPassed.Enabled = false;
                    _rbFailed.Enabled = false;
                    break;
                case AccessCheckFilter.FailureOnly:
                    _cbFilterOnAccess.Checked = true;
                    _rbFailed.Checked = true;
                    break;
            }
            _cbFilterOnAccess.Checked = (inDb.AuditAccessCheck != AccessCheckFilter.NoFilter);

            if (inDb.AuditDmlAll)
            {
                radioAllDML.Checked = true;
            }
            else
            {
                radioSelectedDML.Checked = true;
            }

            //This flag is only based on DML
            if (_chkAuditDML.Checked && ServerRecord.CompareVersions(_server.AgentVersion, "3.5") >= 0)
            {
                _chkCaptureTrans.Enabled = true;
            }
            else
            {
                _chkCaptureTrans.Enabled = false;
                _chkCaptureTrans.Checked = false;
            }

            //This flag is based on DML or SELECT
            if (_chkAuditDML.Checked || _chkAuditSELECT.Checked)
            {
                if (CoreConstants.AllowCaptureSql)
                    _chkCaptureSQL.Enabled = true;
                else
                {
                    _chkCaptureSQL.Enabled = false;
                    _chkCaptureSQL.Checked = false;
                }

                radioAllDML.Enabled = true;
                radioSelectedDML.Enabled = true;
                SetFilterState();
            }
            else
            {
                radioAllDML.Enabled = false;
                radioSelectedDML.Enabled = false;
                grpUserTables.Enabled = false;
                grpUserObjects.Enabled = false;

                _chkCaptureSQL.Enabled = false;
                if (!CoreConstants.AllowCaptureSql)
                    _chkCaptureSQL.Checked = false;
            }

            if (_chkAuditDDL.Checked || _chkAuditSecurity.Checked)
            {
                if (CoreConstants.AllowCaptureSql)
                    chkDBCaptureDDL.Enabled = true;
                else
                {
                    chkDBCaptureDDL.Enabled = false;
                    chkDBCaptureDDL.Checked = false;
                }
            }

            // user tables
            if (inDb.AuditUserTables == 2)
            {
                rbSelectedUserTables.Checked = true;

                // Load User Tables
                _oldTables = DatabaseObjectRecord.GetUserTables(inDb.DbId);
                foreach (DatabaseObjectRecord dbo in _oldTables)
                {
                    ListViewItem x = listTables.Items.Add(SupportsSchemas() ? dbo.FullTableName : dbo.TableName);
                    x.Tag = dbo;
                }
                listTables.Focus(); 
                if (listTables.Items.Count != 0)
                {
                    listTables.TopItem.Selected = true;
                    _btnRemove.Enabled = true;
                }
                else
                {
                    _btnRemove.Enabled = true;
                }
            }
            else
            {
                _oldTables = new ArrayList();
                if (inDb.AuditUserTables == 0)
                {
                    rbDontAuditUserTables.Checked = true;
                }
                else
                {
                    rbAllUserTables.Checked = true;
                }
            }

            listTables.Enabled = rbSelectedUserTables.Checked;
            _btnAddTable.Enabled = rbSelectedUserTables.Checked;

            //Fix for SQLCM-5648
            if (_server.AuditCaptureSQLXE)
            {
                _btnAddTable.Enabled = false;
            }

            _btnRemove.Enabled = rbSelectedUserTables.Checked;


            // Tab: Audited Users
            try
            {
                LoadPrivilegedUsers();
            }
            catch (Exception e)
            {
                MessageBox.Show("Problem loading Privileged User auditing information: " + e, "Error");
            }

            rbAuditUserAll.Checked = inDb.AuditUserAll;
            rbAuditUserSelected.Checked = !inDb.AuditUserAll;

			// SQLCM-5922 - Greyout 'auditUserSelected' if 'auditUserAll' is selected at server level
			if (_server.AuditUserAll)
			{
				rbAuditUserSelected.Enabled = false;
			}

            chkAuditUserLogins.Checked = inDb.AuditUserLogins;
            chkAuditUserLogouts.Checked = inDb.AuditUserLogouts;
            // SQLCM-5375 - Capture Logout Events at Server level
            chkAuditUserLogouts.Checked = inDb.AuditUserLogouts;
            chkAuditUserFailedLogins.Checked = inDb.AuditUserFailedLogins;
            chkAuditUserDDL.Checked = inDb.AuditUserDDL;
            chkAuditUserSecurity.Checked = inDb.AuditUserSecurity;
            chkAuditUserAdmin.Checked = inDb.AuditUserAdmin;
            chkAuditUserDML.Checked = inDb.AuditUserDML;
            chkAuditUserSELECT.Checked = inDb.AuditUserSELECT;
            chkAuditUserUserDefined.Checked = inDb.AuditUserUDE;
            switch (inDb.AuditUserAccessCheck)
            {
                case AccessCheckFilter.FailureOnly:
                    _cbUserFilterAccessCheck.Checked = true;
                    _rbUserAuditFailed.Checked = true;
                    break;
                case AccessCheckFilter.SuccessOnly:
                    _cbUserFilterAccessCheck.Checked = true;
                    _rbUserAuditPassed.Checked = true;
                    break;
                case AccessCheckFilter.NoFilter:
                    _cbUserFilterAccessCheck.Checked = false;
                    _rbUserAuditFailed.Enabled = false;
                    _rbUserAuditPassed.Enabled = false;
                    break;
            }
            chkUserCaptureSQL.Checked = inDb.AuditUserCaptureSQL;
            chkUserCaptureTrans.Checked = inDb.AuditUserCaptureTrans;
            chkUserCaptureDDL.Checked = inDb.AuditUserCaptureDDL;

            //DML only property
            if (rbAuditUserSelected.Checked && chkAuditUserDML.Checked && ServerRecord.CompareVersions(_server.AgentVersion, "3.5") >= 0)
                chkUserCaptureTrans.Enabled = true;
            else
                chkUserCaptureTrans.Enabled = false;

            //DML or SELECT property
            if (rbAuditUserSelected.Checked && (chkAuditUserDML.Checked || chkAuditUserSELECT.Checked) && CoreConstants.AllowCaptureSql)
                chkUserCaptureSQL.Enabled = true;
            else
            {
                chkUserCaptureSQL.Checked = false;
                chkUserCaptureSQL.Enabled = false;
            }

            //DDL or Security property
            if (rbAuditUserSelected.Checked && (chkAuditUserDDL.Checked || chkAuditUserSecurity.Checked) && CoreConstants.AllowCaptureSql)
                chkUserCaptureDDL.Enabled = true;
            else
            {
                chkUserCaptureDDL.Checked = false;
                chkUserCaptureDDL.Enabled = false;
            }

            grpAuditUserActivity.Enabled = !rbAuditUserAll.Checked;

            // other objects
            _chkAuditOther.Checked = inDb.AuditDmlOther;
            _chkAuditStoredProcedures.Checked = inDb.AuditStoredProcedures;
            _chkAuditSystemTables.Checked = inDb.AuditSystemTables;

            try
            {
                // Trusted Users
                LoadTrustedUsers();
            }
            catch (Exception e)
            {
                MessageBox.Show("Problem loading trusted users:  " + e, "Error");
            }

            try
            {
                // Before-After Data
                LoadBeforeAfterConfig();
            }
            catch (Exception e)
            {
                MessageBox.Show("Problem loading Before-After auditing information:  " + e, "Error");
            }

            try
            {
                LoadSensitiveColumns();
            }
            catch (Exception e)
            {
                MessageBox.Show("Problem loading Sensitive Column auditing information: " + e, "Error");
            }
            this.ActiveControl = txtDescription;

            //------------------------------------------------------
            // Make controls read only unless user has admin access
            //------------------------------------------------------
            if (!Globals.isAdmin)
            {
                for (int i = 0; i < _tabControl.TabPages.Count; i++)
                {
                    foreach (Control ctrl in _tabControl.TabPages[i].Controls)
                    {
                        ctrl.Enabled = false;
                    }
                }

                grpStatus.Enabled = true;

                // change buttons
                _btnOK.Visible = false;
                _btnCancel.Text = "Close";
                this.AcceptButton = _btnCancel;
            }

            // reorder the tabs since it's a CF bug
            // by setting the index to "1" it sets it to the top
            // and the existing tabs get "pushed" down
            // this means the tab order on-screen will be in the reverse
            // order that we set here.              |
            _tabControl.Controls.SetChildIndex(this._tabFilters, 1);
            _tabControl.Controls.SetChildIndex(this._tabAuditSettings, 1);
            _tabControl.Controls.SetChildIndex(this._tabGeneral, 1);
        }

        public void SetContext(Context context)
        {
            switch (context)
            {
                case Context.General:
                    _tabControl.SelectedTab = _tabGeneral;
                    break;
                case Context.AuditedActivities:
                   
                    _tabControl.SelectedTab = _tabAuditSettings;
                    break;
                case Context.DmlFilters:
                    _tabControl.SelectedTab = _tabFilters;
                    break;
                case Context.TrustedUsers:
                    _tabControl.SelectedTab = _tabTrustedUsers;
                    break;
                case Context.BeforeAfterData:
                    _tabControl.SelectedTab = _tabBeforeAfter;
                    break;
                case Context.SensitiveColumns:
                    _tabControl.SelectedTab = _tabSensitiveColumns;
                    break;
            }
        }
        //start sqlcm 5.6 - 5364
        public void Movecheck(object sender, MouseEventArgs e)
        {
            Control control = ((Control)sender).GetChildAtPoint(e.Location);

            if (control != null && !control.Enabled)
            {
                if (control is CheckBox)
                {
                    if (_currentToolTipControl == null && ((CheckBox)control).Checked)
                    {

                        tip.Show("This setting has been set up at the Database Level, to disable the setting please uncheck the database level setting.", control, 100, 10);
                        _currentToolTipControl = control;
                    }
                }
            }
            else
            {
                if (_currentToolTipControl != null) tip.Hide(_currentToolTipControl);
                _currentToolTipControl = null;
            }
        }

        //end sqlcm 5.6 - 5364
        private string
            GetDateString(
               DateTime time
          )
        {
            string retStr;

            if (time == DateTime.MinValue)
            {
                retStr = UIConstants.Status_Never;
            }
            else
            {
                DateTime local = time.ToLocalTime();
                retStr = String.Format("{0} {1}",
                                        local.ToShortDateString(),
                                        local.ToShortTimeString());
            }

            return retStr;
        }

        #endregion


        #region OK/Apply/Cancel

        //--------------------------------------------------------------------
        // btnCancel_Click - Close without saving
        //--------------------------------------------------------------------
        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (_sqlServer != null) _sqlServer.CloseConnection();
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        //--------------------------------------------------------------------
        // btnOK_Click - Save and close
        //--------------------------------------------------------------------
        private void btnOK_Click(object sender, EventArgs e)
        {

            if (SaveDatabaseRecord())
            {
                if (_sqlServer != null) _sqlServer.CloseConnection();

                if (_isDirty)
                    this.DialogResult = DialogResult.OK;
                else
                    this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
        }

        #endregion

        #region Private Methods
        //--------------------------------------------------------------------
        // SaveDatabaseRecord
        //--------------------------------------------------------------------
        private bool SaveDatabaseRecord()
        {
            SqlTransaction transaction;
            bool retval = false;
            string errorMsg = "";
            bool isMatching = false; 

            if (ValidateProperties())
            {
                CreateDatabaseRecord();

                //SQLcm 5.6 (Aakash Prakash) - Fix for 4967
                isMatching = DatabaseRecord.Match(this._db, _oldDb);
                if (!isMatching)
                {
                    bool prevEnabled = (_oldDb.AuditDML || _oldDb.AuditSELECT) && (_oldDb.AuditDmlAll || (!_oldDb.AuditDmlAll && _oldDb.AuditUserTables != 2));
                    bool currEnabled = (_db.AuditDML || _db.AuditSELECT) && (_db.AuditDmlAll || (!_db.AuditDmlAll && _db.AuditUserTables != 2));
                    if (prevEnabled && !currEnabled)
                    {
                        ServerRecord.DecrementCountDatabasesAuditingAllObjects(Globals.Repository.Connection, _db.SrvId);
                    }
                    else if(!prevEnabled && currEnabled)
                    {
                        ServerRecord.IncrementCountDatabasesAuditingAllObjects(Globals.Repository.Connection, _db.SrvId);
                    }
                }

                if (!isMatching)
                {
                    try
                    {
                        if (!SaveDatabaseLevelUsers(_db))
                        {
                            errorMsg = DatabaseRecord.GetLastError();
                            throw (new Exception(errorMsg));
                        }
                    }
                    catch (Exception ex)
                    {
                        errorMsg = ex.Message;
                    }
                }
                // Execute Update SQL in a transaction
                using (transaction = Globals.Repository.Connection.BeginTransaction())
                {
                    try
                    {
                        //---------------------------------------
                        // Write Database Properties if necessary
                        //---------------------------------------
                        if (!isMatching)
                        {
                            if (!_db.Write(_oldDb, transaction))
                            {
                                errorMsg = DatabaseRecord.GetLastError();
                                throw (new Exception(errorMsg));
                            }
                            else
                            {
                                _isDirty = true;
                            }
                        }

                        //---------------------------
                        // Write Tables if necessary
                        //---------------------------
                        if (!rbSelectedUserTables.Checked)
                        {
                            if (_oldDb.AuditUserTables == 2)
                            {
                                _isDirty = true;

                                // Delete existing tables - changed from selected to all or none
                                retval = DatabaseObjectRecord.DeleteUserTables(_db.DbId,
                                    transaction);
                            }
                            else
                            {
                                retval = true;
                            }
                        }
                        else
                        {
                            bool same = true;
                            if (_oldTables == null || listTables.Items.Count != _oldTables.Count)
                            {
                                same = false;
                            }
                            else
                            {
                                int i = 0;
                                foreach (DatabaseObjectRecord dbo in _oldTables)
                                {
                                    if (listTables.Items[i].Text != dbo.TableName)
                                    {
                                        same = false;
                                        break;
                                    }
                                    else
                                        i++;
                                }
                            }

                            if (same)
                            {
                                retval = true;
                            }
                            else
                            {
                                _isDirty = true;

                                if (DatabaseObjectRecord.UpdateUserTables(listTables.Items,
                                    _oldTables.Count,
                                    _db.DbId,
                                    transaction))
                                {
                                    retval = true;
                                }
                            }
                        }

                        //Save BAD info
                        if (_oldDb.AuditDataChanges != _db.AuditDataChanges)
                        {
                            // Adding new tables
                            if (_db.AuditDataChanges)
                            {
                                DataChangeTableRecord.CreateUserTables(Globals.Repository.Connection, GetBATables(), _server.SrvId, _oldDb.DbId, transaction);
                            }
                            else
                            {
                                // Removing old tables
                                DataChangeTableRecord.DeleteUserTables(Globals.Repository.Connection, _server.SrvId, _oldDb.DbId, transaction);
                            }
                        }
                        else if (_db.AuditDataChanges)
                        {
                            bool baDirty = false;
                            // Make sure our selected tables didn't change
                            if (_oldBATables.Count != _lvBeforeAfterTables.Items.Count)
                            {
                                baDirty = true;
                            }
                            else
                            {
                                foreach (ListViewItem item in _lvBeforeAfterTables.Items)
                                {
                                    if (_oldBATables.ContainsKey(item.Text))
                                    {
                                        if (SupportsBeforeAfterColumns())
                                        {
                                            if ((_oldBATables[item.Text].RowLimit != Form_TableConfigure.GetMaxRows(item.SubItems[1].Text) ||
                                               (_oldBATables[item.Text].SelectedColumns && item.SubItems[2].Text != UIConstants.BAD_AllColumns) ||
                                               (!_oldBATables[item.Text].SelectedColumns ? UIConstants.BAD_AllColumns : Form_TableConfigure.GetColumnString(_oldBATables[item.Text].Columns)) != item.SubItems[2].Text))
                                                baDirty = true;
                                        }
                                        else if (_oldBATables[item.Text].RowLimit != Form_MaxRows.GetMaxRows(item.SubItems[1].Text))
                                            baDirty = true;
                                    }
                                    else
                                        baDirty = true;
                                }
                            }
                            if (baDirty)
                            {
                                _isDirty = true;
                                DataChangeTableRecord.UpdateUserTables(Globals.Repository.Connection, GetBATables(), _server.SrvId, _oldDb.DbId, transaction);
                            }
                        }

                        //Save Sensitive column info
                        if (_oldDb.AuditSensitiveColumns != _db.AuditSensitiveColumns)
                        {
                            // Adding new tables
                            if (_db.AuditSensitiveColumns)
                            {
                                SensitiveColumnTableRecord.CreateUserTables(Globals.Repository.Connection, GetSCTables(transaction), _server.SrvId, _oldDb.DbId, transaction);
                            }
                            else
                            {
                                // Removing old tables
                                SensitiveColumnTableRecord.DeleteUserTables(Globals.Repository.Connection, _server.SrvId, _oldDb.DbId, transaction);
                            }
                        }
                        else if (_db.AuditSensitiveColumns)
                        {
                            bool scDirty = false;
                            int count = _oldSCTables.Values.Sum(x => x.Count);
                            // Make sure our selected tables didn't change
                            if (count != _lvSCTables.Items.Count)
                            {
                                scDirty = true;
                            }
                            else
                            {
                                foreach (ListViewItem item in _lvSCTables.Items)
                                {
                                    bool isChanged = true;
                                    if (_oldSCTables.ContainsKey(item.Text))
                                    {
                                        string currentItemType = item.SubItems[2].Text;
                                        List<SensitiveColumnTableRecord> value = _oldSCTables[item.Text];
                                        string[] columns = item.SubItems[1].Text.Split(',');
                                        foreach (var co in value)
                                        {
                                            if (co.Type == currentItemType)
                                            {
                                                if (co.Columns.Length == columns.Length)
                                                {
                                                    foreach (string temp in co.Columns)
                                                    {
                                                        if (!columns.Contains(temp))
                                                            break;
                                                        isChanged = false;
                                                    }
                                                }
                                            }
                                        }
                                        if (isChanged)
                                        {
                                            scDirty = isChanged;
                                            break;
                                        }
                                    }

                                    scDirty = isChanged;
                                }
                            }
                            if (scDirty)
                            {
                                _isDirty = true;
                                SensitiveColumnTableRecord.UpdateUserTables(Globals.Repository.Connection, GetSCTables(transaction), _server.SrvId, _oldDb.DbId, transaction);
                            }
                        }
                        if (!retval)
                        {
                            errorMsg = DatabaseObjectRecord.GetLastError();
                        }
                    }
                    catch (Exception ex)
                    {
                        errorMsg = ex.Message;
                    }
                    finally
                    {
                        //-----------------------------------------------------------
                        // Cleanup - Close transaction, update server, display error
                        //-----------------------------------------------------------
                        if (transaction != null)
                        {
                            if (retval && _isDirty)
                            {
                                transaction.Commit();

                                string changeLog = Snapshot.DatabaseChangeLog(Globals.Repository.Connection,
                                    _oldDb,
                                    _db,
                                    _oldTablesSnapshot,
                            _oldDCTablesSnapshot,
                            _oldSCTablesSnapshot);

                                // Register change to server and perform audit log				      
                                ServerUpdate.RegisterChange(_db.SrvId,
                                    LogType.ModifyDatabase,
                                    _db.SrvInstance,
                                    changeLog);
                            }
                            else
                            {
                                transaction.Rollback();
                            }
                        }
                        if (!retval)
                        {
                            ErrorMessage.Show(this.Text,
                                UIConstants.Error_ErrorSavingDatabase,
                                errorMsg);
                        }
                    }
                }
            }
            return retval;
        }

        public bool SaveDatabaseLevelUsers(DatabaseRecord currdb)
        {
            StringBuilder userSQL = new StringBuilder("");
            userSQL.Append(GetDeleteOldUserForCurrentDatabaseSQL(currdb));
            userSQL.Append(GetTrustedUsersSQLForDatabase(currdb));
            userSQL.Append(GetPrivilegedUsersSQLForDatabase(currdb));
            if (currdb.SaveTrustedandPrivUsers(userSQL))
            {
                return true;
            }
            return false;
        }
        private string GetDeleteOldUserForCurrentDatabaseSQL(DatabaseRecord dbDetails)
        {
            string deleteSQL = "";
            deleteSQL += dbDetails.GetDeleteSQLForUser(dbDetails.DbId);
            return deleteSQL;
        }
        private string GetTrustedUsersSQLForDatabase(DatabaseRecord dbDetails)
        {
            string trustedUsersSQL = "";
            foreach (ListViewItem vi in _lstTrustedUsers.Items)
            {
                if (vi.ImageIndex == (int)AppIcons.Img16.Role)
                {
                    trustedUsersSQL += dbDetails.GetInsertSQLForUser(0, dbDetails.DbId, 1, 0, vi.Text, null);
                }
                else
                {
                    trustedUsersSQL += dbDetails.GetInsertSQLForUser(0, dbDetails.DbId, 1, 0, null, vi.Text);
                }
            }
            return trustedUsersSQL;
        }
        private string GetPrivilegedUsersSQLForDatabase(DatabaseRecord dbDetails)
        {
            string privUsersSQL = "";
            foreach (ListViewItem vi in lstPrivilegedUsers.Items)
            {
                if (vi.ImageIndex == (int)AppIcons.Img16.Role)
                {
                    privUsersSQL += dbDetails.GetInsertSQLForUser(0, dbDetails.DbId, 0, 1, vi.Text, null);
                }
                else
                {
                    privUsersSQL += dbDetails.GetInsertSQLForUser(0, dbDetails.DbId, 0, 1, null, vi.Text);
                }
            }
            return privUsersSQL;
        }
        //--------------------------------------------------------------------
        // ValidateProperties
        //--------------------------------------------------------------------
        private bool ValidateProperties()
        {
            // audit settings - make sure something checked
            if (!_chkAuditSecurity.Checked &&
                  !_chkAuditDDL.Checked &&
                  !_chkAuditAdmin.Checked &&
                  !_chkAuditDML.Checked &&
                  !_chkAuditSELECT.Checked)
            {
                ErrorMessage.Show(this.Text,
                                     UIConstants.Error_MustSelectOneAuditOption);

                _tabControl.SelectedTab = _tabAuditSettings;
                _chkAuditSecurity.Focus();

                return false;
            }


            if (_chkAuditDML.Checked || _chkAuditSELECT.Checked)
            {
                if (radioSelectedDML.Checked)
                {
                    // make sure something selected for auditing
                    if (rbDontAuditUserTables.Checked &&
                          !_chkAuditSystemTables.Checked &&
                          !_chkAuditStoredProcedures.Checked &&
                          !_chkAuditOther.Checked)
                    {
                        ErrorMessage.Show(this.Text,
                                             UIConstants.Error_MustSelectOneAuditObject);
                        _tabControl.SelectedTab = _tabFilters;
                        radioSelectedDML.Focus();
                        return false;
                    }

                    // tables
                    if (rbSelectedUserTables.Checked)
                    {
                        if (listTables.Items.Count == 0)
                        {
                            ErrorMessage.Show(this.Text,
                                              UIConstants.Error_NoUserTables);

                            _tabControl.SelectedTab = _tabFilters;
                            rbSelectedUserTables.Focus();

                            return false;
                        }
                    }
                }
            }

            // BAD tables
            if (_lvBeforeAfterTables.Items.Count > 0)
            {
                if (_chkAuditDML.Checked)
                {
                    if (!rbDontAuditUserTables.Checked)
                    {
                        bool notConfigured = false;
                        foreach (ListViewItem lvi in _lvBeforeAfterTables.Items)
                        {
                            if (rbSelectedUserTables.Checked)
                            {
                                // if only some tables are audited, then make sure the BAD table is being audited
                                bool tblIsAudited = false;
                                foreach (ListViewItem tbl in listTables.Items)
                                {
                                    if (lvi.SubItems[0].Text == tbl.Text)
                                    {
                                        tblIsAudited = true;
                                        break;
                                    }
                                }

                                if (!tblIsAudited)
                                {
                                    _tabControl.SelectedTab = _tabBeforeAfter;
                                    ErrorMessage.Show(this.Text,
                                                      string.Format(UIConstants.Error_BADTableNotAudited, lvi.SubItems[0].Text));

                                    _lvBeforeAfterTables.Focus();

                                    return false;
                                }
                            }
                            if (SupportsBeforeAfterColumns())
                            {
                                if (lvi.SubItems[2].Text == UIConstants.BAD_NoColumns)
                                {
                                    notConfigured = true;
                                    _lvBeforeAfterTables.SelectedItems.Clear();
                                    lvi.Selected = true;
                                    break;
                                }
                            }
                            if (lvi.Tag == null)
                            {
                                _tabControl.SelectedTab = _tabBeforeAfter;
                                if (DialogResult.No == MessageBox.Show(UIConstants.Warning_BAD_Tables_Removed, this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                                {
                                    return false;
                                }
                            }
                        }

                        if (notConfigured)
                        {
                            _tabControl.SelectedTab = _tabBeforeAfter;
                            ErrorMessage.Show(this.Text,
                                              UIConstants.Error_BlobTablesNotConfigured);

                            _lvBeforeAfterTables.Focus();

                            return false;
                        }
                    }
                    else
                    {
                        _tabControl.SelectedTab = _tabFilters;
                        ErrorMessage.Show(this.Text,
                                          UIConstants.Error_UserTableAuditingNotEnabled);

                        _lvBeforeAfterTables.Focus();

                        return false;
                    }
                }
                else
                {
                    _tabControl.SelectedTab = _tabAuditSettings;
                    ErrorMessage.Show(this.Text,
                                      UIConstants.Error_DMLAuditingNotEnabled);

                    _lvBeforeAfterTables.Focus();

                    return false;
                }
            }

            // privileged users
            if (lstPrivilegedUsers.Items.Count > 0 &&
                   rbAuditUserSelected.Checked)
            {
                // make sure something checked
                if (!chkAuditUserLogins.Checked &&
                   !chkAuditUserFailedLogins.Checked &&
                   !chkAuditUserSecurity.Checked &&
                   !chkAuditUserAdmin.Checked &&
                   !chkAuditUserDDL.Checked &&
                   !chkAuditUserDML.Checked &&
                   !chkAuditUserSELECT.Checked &&
                   !chkAuditUserUserDefined.Checked)
                {
                    ErrorMessage.Show(this.Text,
                                      UIConstants.Error_MustSelectOneAuditUserOption);
                    _tabControl.SelectedTab = _tabPrivilegedUser;
                    chkAuditUserLogins.Focus();
                    return false;
                }
            }

            return true;
        }

        private List<DataChangeTableRecord> GetBATables()
        {
            List<DataChangeTableRecord> retVal = new List<DataChangeTableRecord>();
            LoadTables();
            foreach (ListViewItem item in _lvBeforeAfterTables.Items)
            {
                DatabaseObjectRecord dor = (DatabaseObjectRecord)item.Tag;
                // Check and make sure the table still exists for auditing or skip it which will remove it from auditing
                if (dor != null)
                {
                    DataChangeTableRecord dctItem = new DataChangeTableRecord();
                    dctItem.SchemaName = dor.SchemaName;
                    dctItem.TableName = dor.TableName;
                    dctItem.ObjectId = dor.Id;

                    if (!SupportsBeforeAfterColumns())
                    {
                        dctItem.RowLimit = Form_MaxRows.GetMaxRows(item.SubItems[1].Text);
                        dctItem.SelectedColumns = false;
                    }
                    else if (item.SubItems[2].Text == UIConstants.BAD_AllColumns)
                    {
                        dctItem.RowLimit = Form_TableConfigure.GetMaxRows(item.SubItems[1].Text);
                        dctItem.SelectedColumns = false;
                    }
                    else
                    {
                        dctItem.RowLimit = Form_TableConfigure.GetMaxRows(item.SubItems[1].Text);
                        dctItem.SelectedColumns = true;
                        foreach (string col in Form_TableConfigure.GetColumns(item.SubItems[2].Text))
                        {
                            dctItem.AddColumn(col);
                        }
                    }
                    retVal.Add(dctItem);
                }
            }

            return retVal;
        }

        //--------------------------------------------------------------------
        // CreateDatabaseRecord
        //--------------------------------------------------------------------
        private void
           CreateDatabaseRecord()
        {
            _db = _oldDb.Clone();

            _db.Connection = Globals.Repository.Connection;

            // General
            _db.Description = txtDescription.Text;

            // Audit Settings		
            
            _db.AuditDDL = _chkAuditDDL.Checked;
            _db.AuditSecurity = _chkAuditSecurity.Checked;
            _db.AuditAdmin = _chkAuditAdmin.Checked;
            _db.AuditDML = _chkAuditDML.Checked;
            _db.AuditSELECT = _chkAuditSELECT.Checked;
            if (_cbFilterOnAccess.Checked)
            {
                if (_rbFailed.Checked)
                    _db.AuditAccessCheck = AccessCheckFilter.FailureOnly;
                else
                    _db.AuditAccessCheck = AccessCheckFilter.SuccessOnly;
            }
            else
            {
                _db.AuditAccessCheck = AccessCheckFilter.NoFilter;
            }
            _db.AuditCaptureSQL = _chkCaptureSQL.Checked;
            _db.AuditCaptureTrans = _chkCaptureTrans.Checked;
            _db.AuditExceptions = _oldDb.AuditExceptions;
            _db.AuditUsersList = GetTrustedUserProperty();
            _db.AuditCaptureDDL = chkDBCaptureDDL.Checked;

            if (_db.AuditDML || _db.AuditSELECT)
            {
                if (radioAllDML.Checked)
                {
                    _db.AuditDmlAll = true;
                }
                else
                {
                    _db.AuditDmlAll = false;

                    // User Tables
                    if (rbDontAuditUserTables.Checked)
                        _db.AuditUserTables = 0;
                    else if (rbAllUserTables.Checked)
                        _db.AuditUserTables = 1;
                    else
                        _db.AuditUserTables = 2;

                    _db.AuditSystemTables = _chkAuditSystemTables.Checked;
                    _db.AuditStoredProcedures = _chkAuditStoredProcedures.Checked;
                    _db.AuditDmlOther = _chkAuditOther.Checked;
                }
            }
     
            if (_db.AuditDML || _db.AuditSELECT)
            {
                if (radioAllDML.Checked)
                {
                    _db.AuditDmlAll = true;
                }
                else
                {
                    _db.AuditDmlAll = false;

                    // User Tables
                    if (rbDontAuditUserTables.Checked)
                        _db.AuditUserTables = 0;
                    else if (rbAllUserTables.Checked)
                        _db.AuditUserTables = 1;
                    else
                        _db.AuditUserTables = 2;

                    _db.AuditSystemTables = _chkAuditSystemTables.Checked;
                    _db.AuditStoredProcedures = _chkAuditStoredProcedures.Checked;
                    _db.AuditDmlOther = _chkAuditOther.Checked;
                }
            }

            //BAD
            if (_lvBeforeAfterTables.Items.Count > 0 && _db.AuditDML)
                _db.AuditDataChanges = true;
            else
                _db.AuditDataChanges = false;

            //Sensitive Columns
            if (_lvSCTables.Items.Count > 0)
                _db.AuditSensitiveColumns = true;
            else
                _db.AuditSensitiveColumns = false;

            // Privileged users
            _db.AuditPrivUsersList = GetPrivilegedUserProperty();
            _db.AuditUserAll = rbAuditUserAll.Checked;
            _db.AuditUserLogins = chkAuditUserLogins.Checked;
            // SQLCM-5375 - Capture Logout Events at Server level
            _db.AuditUserLogouts = chkAuditUserLogouts.Checked;
            _db.AuditUserFailedLogins = chkAuditUserFailedLogins.Checked;
            _db.AuditUserDDL = chkAuditUserDDL.Checked;
            _db.AuditUserSecurity = chkAuditUserSecurity.Checked;
            _db.AuditUserAdmin = chkAuditUserAdmin.Checked;
            _db.AuditUserDML = chkAuditUserDML.Checked;
            _db.AuditUserSELECT = chkAuditUserSELECT.Checked;
            _db.AuditUserUDE = chkAuditUserUserDefined.Checked;
            if (_cbUserFilterAccessCheck.Checked)
            {
                if (_rbUserAuditFailed.Checked)
                    _db.AuditUserAccessCheck = AccessCheckFilter.FailureOnly;
                else
                    _db.AuditUserAccessCheck = AccessCheckFilter.SuccessOnly;
            }
            else
            {
                _db.AuditUserAccessCheck = AccessCheckFilter.NoFilter;
            }
            _db.AuditUserCaptureSQL = chkUserCaptureSQL.Checked;
            _db.AuditUserCaptureTrans = chkUserCaptureTrans.Checked;
            _db.AuditUserCaptureDDL = chkUserCaptureDDL.Checked;
        }

        #endregion

        #region Form Event Handlers

        //--------------------------------------------------------------------
        // chkCaptureSQL_CheckedChanged
        //--------------------------------------------------------------------
        private void chkCaptureSQL_CheckedChanged(object sender, EventArgs e)
        {
            if (_isLoaded && _chkCaptureSQL.Checked)
            {
                ErrorMessage.Show(this.Text,
                                     UIConstants.Warning_CaptureAll,
                                     "",
                                     MessageBoxIcon.Warning);
            }
        }

        #endregion

        #region User Table Page Handlers

        //--------------------------------------------------------------------
        // btnAddTable_Click - Select tables for auditing from browse list
        //--------------------------------------------------------------------
        private void btnAddTable_Click(object sender, EventArgs e)
        {
            // Attempt to load list of tables
            if (!LoadTables())
            {
                ErrorMessage.Show(this.Text,
                                  UIConstants.Error_CantLoadTables,
                                  SQLDirect.GetLastError());
                _btnAddTable.Enabled = false;
                return;
            }
            List<string> selectedTables = new List<string>();

            foreach (ListViewItem tableItem in listTables.Items)
            {
               
                    selectedTables.Add(tableItem.Text);
            }

            Form_TableAdd frm = new Form_TableAdd(_tableList, selectedTables, SupportsSchemas(), UIConstants.Table_Column_Usage.Filter);
            if (DialogResult.OK == frm.ShowDialog())
            {
              
                listTables.BeginUpdate();
                listTables.Items.Clear();

                foreach (string tableName in frm.SelectedTables)
                {
                    ListViewItem x = listTables.Items.Add(tableName);
                    try
                    {
                        x.Tag = _tableObjects[tableName];
                    }
                   //Start Sqlcm 5.6 (Adding try catch to remove duplicate tables not available for auditing)
                    catch (KeyNotFoundException ex)
                    {
                        if(!_tableObjects.ContainsKey(tableName))
                        {
                        MessageBox.Show(String.Format(UIConstants.BeforeAfterAuditing_DuplicateErrorMessage,tableName));
                           listTables.Items.Remove(x);
                        }
                    }
                    //End Sqlcm 5.6 
               
                }

                listTables.Focus();
                if (listTables.Items.Count > 0)
                {
                    listTables.TopItem.Selected = Globals.isAdmin;
                    _btnRemove.Enabled = Globals.isAdmin;
                }
                else
                {
                    _btnRemove.Enabled = false;
                }

                listTables.EndUpdate();
            }
        }

        //--------------------------------------------------------------------
        // btnRemove_Click - Remove selected tables
        //--------------------------------------------------------------------
        private void btnRemove_Click(object sender, EventArgs e)
        {
            listTables.BeginUpdate();

            int ndx = listTables.SelectedItems[0].Index;


            foreach (ListViewItem table in listTables.SelectedItems)
            {
                table.Remove();
            }

            listTables.EndUpdate();

            // reset selected item
            if (Globals.isAdmin && listTables.Items.Count != 0)
            {
                listTables.Focus();
                if (ndx >= listTables.Items.Count)
                {
                    listTables.Items[listTables.Items.Count - 1].Selected = true;
                }
                else
                    listTables.Items[ndx].Selected = true;
            }
        }

        //--------------------------------------------------------------------
        // rbSelectedUserTables_CheckedChanged
        //--------------------------------------------------------------------
        private void rbSelectedUserTables_CheckedChanged(object sender, EventArgs e)
        {
            if (rbSelectedUserTables.Checked)
            {
                //Prevent user from selecting specific tables when auditing is done via extended events
                if (LoadTables() && !_server.AuditCaptureSQLXE) 
                {
                    _btnAddTable.Enabled = true;
                }
                else
                {
                    _btnAddTable.Enabled = false;
                }
            }
            else
            {
                _btnAddTable.Enabled = false;
            }

            if (listTables.SelectedItems.Count > 0)
                _btnRemove.Enabled = Globals.isAdmin && rbSelectedUserTables.Checked;
            else
                _btnRemove.Enabled = false;

            listTables.Enabled = rbSelectedUserTables.Checked;
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
            if (!radioAllDML.Checked && rbDontAuditUserTables.Checked && _pnlBeforeAfter.Enabled && SupportsBeforeAfter())
            {
                _pnlBeforeAfter.Visible = false;
                _lblBeforeAfterStatus.Text = CoreConstants.Feature_BeforeAfterNotAvailableUserTables;
            }
            else
            {
                _pnlBeforeAfter.Visible = true;
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
        }

        #endregion

        #region Help
        //--------------------------------------------------------------------
        // Form_DatabaseProperties_HelpRequested - Show Context Sensitive Help
        //--------------------------------------------------------------------
        private void Form_DatabaseProperties_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            string helpTopic;

            if (_tabControl.SelectedTab == _tabFilters)
                helpTopic = HelpAlias.SSHELP_Form_DatabaseProperties_Objects;
            else if (_tabControl.SelectedTab == _tabAuditSettings)
                helpTopic = HelpAlias.SSHELP_Form_DatabaseProperties_Activities;
            else if (_tabControl.SelectedTab == _tabTrustedUsers)
                helpTopic = HelpAlias.SSHELP_Form_DatabaseProperties_TrustedUsers;
            else if (_tabControl.SelectedTab == _tabBeforeAfter)
                helpTopic = HelpAlias.SSHELP_Form_DatabaseProperties_BeforeAfterData;
            else if (_tabControl.SelectedTab == _tabSensitiveColumns)
                helpTopic = HelpAlias.SSHELP_Form_DatabaseProperties_SensitiveColumns;
            else
                helpTopic = HelpAlias.SSHELP_Form_DatabaseProperties_General;

            HelpAlias.ShowHelp(this, helpTopic);
            hlpevent.Handled = true;
        }
        #endregion

        #region DML/Select Filters

        private void chkAuditDML_CheckedChanged(object sender, EventArgs e)
        {
            _pnlBeforeAfter.Enabled = _chkAuditDML.Checked;

            //DML only flag
            if (_chkAuditDML.Checked && ServerRecord.CompareVersions(_server.AgentVersion, "3.5") >= 0)
            {
                _chkCaptureTrans.Enabled = Globals.isAdmin;
            }
            else
            {
                _chkCaptureTrans.Enabled = false;
                _chkCaptureTrans.Checked = false;
            }

            //DML and SELECT flag
            if (_chkAuditDML.Checked || _chkAuditSELECT.Checked)
            {
                if (CoreConstants.AllowCaptureSql)
                    _chkCaptureSQL.Enabled = Globals.isAdmin;
                else
                {
                    _chkCaptureSQL.Enabled = false;
                    _chkCaptureSQL.Checked = false;
                }

                radioAllDML.Enabled = Globals.isAdmin;
                radioSelectedDML.Enabled = Globals.isAdmin;
                SetFilterState();
            }
            else
            {
                radioAllDML.Enabled = false;
                radioSelectedDML.Enabled = false;
                grpUserTables.Enabled = false;
                grpUserObjects.Enabled = false;

                _chkCaptureSQL.Enabled = false;
            }
            UpdateBeforeAfterAvailability();
        }

        private void listTables_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listTables.SelectedItems.Count != 0)
                _btnRemove.Enabled = Globals.isAdmin;
            else
                _btnRemove.Enabled = false;
        }

        private void radioAllDML_CheckedChanged(object sender, EventArgs e)
        {
            SetFilterState();
            UpdateBeforeAfterAvailability();
        }

        private void SetFilterState()
        {
            grpUserTables.Enabled = radioSelectedDML.Checked;
            grpUserObjects.Enabled = radioSelectedDML.Checked;
        }

        private void Click_ChkExcludeFailedAccess(object sender, EventArgs e)
        {
            if (_cbFilterOnAccess.Checked)
            {
                _rbPassed.Enabled = true;
                _rbFailed.Enabled = true;
            }
            else
            {
                _rbPassed.Enabled = false;
                _rbFailed.Enabled = false;
            }
        }
        #endregion

        #region Trusted Users
        private void Click_btnAddUser(object sender, EventArgs e)
        {
            Form_PrivUser frm = new Form_PrivUser(_oldDb.SrvInstance, false);
            //frm.MainForm = this.mainForm;                                                      
            if (DialogResult.OK == frm.ShowDialog())
            {
                _lstTrustedUsers.BeginUpdate();

                _lstTrustedUsers.SelectedItems.Clear();

                foreach (ListViewItem itm in frm.listSelected.Items)
                {
                    bool found = false;
                    foreach (ListViewItem s in _lstTrustedUsers.Items)
                    {
                        if (itm.Text == s.Text)
                        {
                            found = true;
                            s.Selected = true;
                            break;
                        }
                    }

                    if (!found)
                    {
                        ListViewItem newItem = new ListViewItem(itm.Text);
                        newItem.Tag = itm.Tag;
                        newItem.ImageIndex = itm.ImageIndex;
                        _lstTrustedUsers.Items.Add(newItem);
                    }
                }

                _lstTrustedUsers.EndUpdate();

                if (_lstTrustedUsers.Items.Count > 0)
                {
                    _lstTrustedUsers.TopItem.Selected = true;
                    _btnRemoveUser.Enabled = true;
                }
                else
                {
                    _btnRemoveUser.Enabled = false;
                }
            }
        }

        private void Click_btnRemoveUser(object sender, EventArgs e)
        {
            if (_lstTrustedUsers.SelectedItems.Count == 0)
            {
                _btnRemoveUser.Enabled = false;
                return;
            }

            _lstTrustedUsers.BeginUpdate();

            int ndx = _lstTrustedUsers.SelectedIndices[0];

            foreach (ListViewItem priv in _lstTrustedUsers.SelectedItems)
            {
                priv.Remove();
            }

            _lstTrustedUsers.EndUpdate();

            // reset selected item
            if (_lstTrustedUsers.Items.Count != 0)
            {
                _lstTrustedUsers.Focus();
                if (ndx >= _lstTrustedUsers.Items.Count)
                {
                    _lstTrustedUsers.Items[_lstTrustedUsers.Items.Count - 1].Selected = true;
                }
                else
                    _lstTrustedUsers.Items[ndx].Selected = true;
            }
            else
            {
                _btnRemoveUser.Enabled = false;
            }
        }

        private void SelectedIndexChanged_lstTrustedUsers(object sender, EventArgs e)
        {
            if (_lstTrustedUsers.SelectedItems.Count == 0)
            {
                _btnRemoveUser.Enabled = false;
            }
            else
            {
                _btnRemoveUser.Enabled = Globals.isAdmin;
                // v5.6 SQLCM-5373 - disable remove button if the trusted user is set at server level
                UserList userList = new UserList(_server.AuditTrustedUsersList);
                string selectedText = _lstTrustedUsers.SelectedItems[0].Text;

                foreach (Login l in userList.Logins)
                {
                    if (selectedText.Equals(l.Name))
                    {
                        _btnRemoveUser.Enabled = false;
                        break;
                    }
                }
                foreach (ServerRole l in userList.ServerRoles)
                {
                    if (selectedText.Equals(l.Name))
                    {
                        _btnRemoveUser.Enabled = false;
                        break;
                    }
                }

                foreach (ListViewItem priv in _lstTrustedUsers.SelectedItems)
                {
                    if (priv.ForeColor == System.Drawing.Color.Gray)
                    {
                        priv.Selected = false;
                    }
                }
            }
        }

        private void LoadTrustedUsers()
        {
            if (!SupportsTrustedUsers())
            {
                _lblTrustedUserStatus.Text = CoreConstants.Feature_TrustedUserNotAvailableAgent;
                _pnlTrustedUsers.Visible = false;
                return;
            }
            _lstTrustedUsers.BeginUpdate();
            UserList serverTrustedUsers = new UserList(_server.AuditTrustedUsersList);
            UserList userList = new UserList(_oldDb.AuditUsersList);

            // Add logins
            foreach (Login l in userList.Logins)
            {
                ListViewItem vi = _lstTrustedUsers.Items.Add(l.Name);
                vi.Tag = l.Sid;
                vi.ImageIndex = (int)AppIcons.Img16.WindowsUser;
                if(serverTrustedUsers.Logins.Any(serverTrustedUser => serverTrustedUser.Name == l.Name))
                {
                    vi.ForeColor = System.Drawing.Color.Gray;
                    vi.UseItemStyleForSubItems = true;
                }
            }

            // Add server roles
            foreach (ServerRole r in userList.ServerRoles)
            {
                ListViewItem vi = _lstTrustedUsers.Items.Add(r.FullName);
                vi.Tag = r.Id;
                vi.ImageIndex = (int)AppIcons.Img16.Role;
                // SQLCM-5868: Roles added to default server settings gets added twice at database level
                if (serverTrustedUsers.ServerRoles.Any(serverTrustedUser => serverTrustedUser.CompareName(r)))
                {
                    vi.ForeColor = System.Drawing.Color.Gray;
                    vi.UseItemStyleForSubItems = true;
                }
            }

            _lstTrustedUsers.EndUpdate();

            if (_lstTrustedUsers.Items.Count > 0)
            {
                _lstTrustedUsers.TopItem.Selected = Globals.isAdmin;
                _btnRemoveUser.Enabled = Globals.isAdmin;
            }
            else
            {
                _btnRemoveUser.Enabled = false;
            }
        }

        private string GetTrustedUserProperty()
        {
            int count = 0;

            UserList ul = new UserList();

            foreach (ListViewItem vi in _lstTrustedUsers.Items)
            {
                count++;
                if (vi.ImageIndex == (int)AppIcons.Img16.Role)
                {
                    ul.AddServerRole(vi.Text, vi.Text, (int)vi.Tag);
                }
                else
                {
                    ul.AddLogin(vi.Text, (byte[])vi.Tag);
                }
            }

            return (count == 0) ? "" : ul.ToString();
        }

        private void LinkClicked_lnkTrustedUserHelp(object sender, LinkLabelLinkClickedEventArgs e)
        {
            HelpAlias.ShowHelp(this, HelpAlias.SSHELP_Form_DatabaseProperties_TrustedUsers);
        }

        #endregion

        #region Before After Data
        private void LoadBeforeAfterConfig()
        {
            _beforeAfterAvailable = true;
            // SQL Server 2005,2008
            if (_server.SqlVersion == 0)
            {
                _lblBeforeAfterStatus.Text = CoreConstants.Feature_BeforeAfterNotAvailableVersionUnknown;
                _pnlBeforeAfter.Visible = false;
                _beforeAfterAvailable = false;
                return;
            }
            else if (_server.SqlVersion < 9)
            {
                _lblBeforeAfterStatus.Text = CoreConstants.Feature_BeforeAfterNotAvailable;
                _pnlBeforeAfter.Visible = false;
                _beforeAfterAvailable = false;
                return;
            }
            else if (_compatibilityLevel < 90)
            {
                _lblBeforeAfterStatus.Text = CoreConstants.Feature_BeforeAfterNotAvailableCompatibility;
                _pnlBeforeAfter.Visible = false;
                _beforeAfterAvailable = false;
                return;
            }
            else if (!SupportsBeforeAfter())
            {
                _lblBeforeAfterStatus.Text = CoreConstants.Feature_BeforeAfterNotAvailableAgent;
                _pnlBeforeAfter.Visible = false;
                _beforeAfterAvailable = false;
                return;
            }
            if (!LoadTables())
            {
                _lblBeforeAfterStatus.Text = UIConstants.Error_CantLoadTables;
                _pnlBeforeAfter.Visible = false;
                _beforeAfterAvailable = false;
                return;
            }
            else
            {
                UpdateClrStatus();
            }
            _pnlBeforeAfter.Enabled = _oldDb.AuditDML;

            
            //Load the BAD info
            _oldBATables = new Dictionary<string, DataChangeTableRecord>();
            if (_oldDb.AuditDataChanges)
            {
                List<string> missingTables = new List<string>();
                List<DataChangeTableRecord> tables = DataChangeTableRecord.GetAuditedTables(Globals.Repository.Connection, _server.SrvId, _oldDb.DbId);
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
            SelectedIndexChanged_lvBeforeAfterTables(null, null);
            UpdateBeforeAfterAvailability();

            
        }
        private void SelectedIndexChanged_lvBeforeAfterTables(object sender, EventArgs e)
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

        private void Click_btnAddBATable(object sender, EventArgs e)
        {
            if (!LoadTables())
            {
                ErrorMessage.Show(this.Text,
                                  UIConstants.Error_CantLoadTables,
                                  SQLDirect.GetLastError());
                _btnAddTable.Enabled = false;
                return;
            }
            List<string> selectedTables = new List<string>();
            List<RawTableObject> tableList = new List<RawTableObject>();
            if (SupportsBeforeAfterColumns())
            {
                foreach (ListViewItem tableItem in _lvBeforeAfterTables.Items)
                    selectedTables.Add(tableItem.Text);
                if (_chkAuditDML.Checked && radioSelectedDML.Checked && rbSelectedUserTables.Checked)
                {
                    foreach (RawTableObject rto in _tableList)
                    {
                        foreach (ListViewItem item in listTables.Items)
                        {
                            if (item.Text.Equals(rto.FullTableName))
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
                    ErrorMessage.Show(this.Text,
                                      UIConstants.Error_CantLoadTables,
                                      ex.Message);
                    _btnAddTable.Enabled = false;
                    return;
                }

                foreach (ListViewItem tableItem in _lvBeforeAfterTables.Items)
                    selectedTables.Add(tableItem.Text);
                if (_chkAuditDML.Checked && radioSelectedDML.Checked && rbSelectedUserTables.Checked)
                {
                    foreach (RawTableObject rto in _tableList)
                    {
                        foreach (ListViewItem item in listTables.Items)
                        {
                            if (item.Text.Equals(rto.FullTableName) &&
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
            //Sqlcm 5.6  Start(setting duplicates table list in form add dialog box)
            if (duplicates != null)
                frm.Duplicates = duplicates;
            //Sqlcm 5.6  End
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

        private bool LoadTables()
        {
            // Attempt to load list of tables if we haven't tried already
            if (_tableList == null && _tableObjects == null)
            {
                _tableObjects = new Dictionary<string, DatabaseObjectRecord>();
                // try via connection to agent
                if (_tryAgentCommunication)
                {
                    string url = EndPointUrlBuilder.GetUrl(typeof(AgentManager), Globals.SQLcomplianceConfig.Server, Globals.SQLcomplianceConfig.ServerPort);
                    try
                    {
                        url = String.Format("tcp://{0}:{1}/{2}",
                                             Globals.SQLcomplianceConfig.Server,
                                             Globals.SQLcomplianceConfig.ServerPort,
                                             typeof(AgentManager).Name);
                        AgentManager manager = GUIRemoteObjectsProvider.AgentManager();

                        _tableList = manager.GetRawTables(txtServer.Text, txtName.Text);
                    }
                    catch (Exception ex)
                    {
                        ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                                String.Format("LoadTables: URL: {0} Instance {1} Database {2}", url, txtServer.Text, txtName.Text),
                                                ex,
                                                ErrorLog.Severity.Warning);
                        _tableList = null;
                    }
                }

                // straight connection to SQL Server
                if (_tableList == null)
                {
                    if (_sqlServer.OpenConnection(txtServer.Text))
                    {
                        _tableList = RawSQL.GetTables(_sqlServer.Connection, txtName.Text);
                    }
                }

                bool supportsSchemas = SupportsSchemas();

               
                if (_tableList != null)
                {
                    foreach (RawTableObject rto in _tableList)
                    {
                        DatabaseObjectRecord dbo = new DatabaseObjectRecord(rto);
                        dbo.DbId = _oldDb.DbId;
                        
                         
                        try
                        {
                            if (supportsSchemas)
                            {
                                //start sqlcm 5.6 - 5635
                                if (dbo.TableName.StartsWith(dbo.SchemaName))
                                    _tableObjects.Add(dbo.SchemaName + "." + dbo.TableName, dbo);
                                //end sqlcm 5.6 - 5635
                                else
                                    _tableObjects.Add(dbo.FullTableName, dbo);
                            }
                            else
                                _tableObjects.Add(dbo.TableName, dbo);
                        }
                       //Start SQLCM 5.6 (Adding try catch block to handle duplicate values)
                        catch (ArgumentException ex)
                        {
                            MessageBox.Show(ex+"");
                        }
                        //End SQLCM 5.6
                    } 
                       
                   }
            }
            return (_tableList != null && _tableObjects != null);
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
                    AgentManager manager = GUIRemoteObjectsProvider.AgentManager();

                    columnList = manager.GetRawColumns(txtServer.Text, txtName.Text, tableName);
                }
                catch (Exception ex)
                {
                    ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                            String.Format("LoadColumns: URL: {0} Instance {1} Database {2} Table {3}", url, txtServer.Text, txtName.Text, tableName),
                                            ex,
                                            ErrorLog.Severity.Warning);
                    columnList = null;
                }
            }

            // straight connection to SQL Server
            if (columnList == null)
            {
                if (_sqlServer.OpenConnection(txtServer.Text))
                {
                    columnList = RawSQL.GetColumns(_sqlServer.Connection, txtName.Text, tableName);
                }
            }
            return columnList;
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
                ErrorLog.Instance.Write(String.Format("Unable to contact {0} to determine CLR Enabled status.", txtServer.Text),
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
                string url = EndPointUrlBuilder.GetUrl(typeof(AgentManager), Globals.SQLcomplianceConfig.Server, Globals.SQLcomplianceConfig.ServerPort);
                try
                {
                    AgentManager manager = GUIRemoteObjectsProvider.AgentManager(); //TODO log url

                    manager.EnableCLR(_server.Instance, enable);
                    return;
                }
                catch (Exception ex)
                {
                    ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                            String.Format("EnableClr: URL: {0} Instance {1} Database {2}", url, txtServer.Text, txtName.Text),
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
                string url = EndPointUrlBuilder.GetUrl(typeof(AgentManager), Globals.SQLcomplianceConfig.Server, Globals.SQLcomplianceConfig.ServerPort);
                try
                {
                    AgentManager manager = GUIRemoteObjectsProvider.AgentManager();

                    manager.GetCLRStatus(_server.Instance, out configured, out running);
                    return;
                }
                catch (Exception ex)
                {
                    ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                            String.Format("IsCLREnabled: URL: {0} Instance {1} Database {2}", url, txtServer.Text, txtName.Text),
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

        // Returns true if the agent is able to support trusted users (3.0 and beyond)
        private bool SupportsTrustedUsers()
        {
            if (_server == null ||
               String.IsNullOrEmpty(_server.AgentVersion) ||
               _server.AgentVersion.StartsWith("1") ||
               _server.AgentVersion.StartsWith("2"))
                return false;
            else
                return true;
        }

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

        private bool SupportsSchemas()
        {
            if (_server == null || _server.SqlVersion < 9)
                return false;

            return SupportsBeforeAfter();
        }

        private int GetCompatibilityLevel()
        {
            int retVal;
            // Try agent first if allowed
            if (_tryAgentCommunication)
            {
                string url = EndPointUrlBuilder.GetUrl(typeof(AgentManager), Globals.SQLcomplianceConfig.Server, Globals.SQLcomplianceConfig.ServerPort);
                try
                {
                    AgentManager manager = GUIRemoteObjectsProvider.AgentManager();

                    return manager.GetCompatibilityLevel(_server.Instance, _oldDb.Name);
                }
                catch (Exception ex)
                {
                    ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                            String.Format("GetCompatibilityLevel: URL: {0} Instance {1} Database {2}", url, txtServer.Text, txtName.Text),
                                            ex,
                                            ErrorLog.Severity.Warning);
                }
            }
            try
            {
                // Now we try direct connect
                if (!_sqlServer.OpenConnection(_server.Instance))
                    throw new Exception("Unable to open a connection to server.");

                retVal = RawSQL.GetCompatibilityLevel(_sqlServer.Connection, _oldDb.Name);
                _sqlServer.CloseConnection();
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Default,
                                           String.Format("GetCompatibilityLevel Direct: Instance {0} Database {1}", txtServer.Text, txtName.Text),
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
                string url = EndPointUrlBuilder.GetUrl(typeof(AgentManager), Globals.SQLcomplianceConfig.Server, Globals.SQLcomplianceConfig.ServerPort);
                try
                {
                    AgentManager manager = GUIRemoteObjectsProvider.AgentManager();

                    return manager.GetBlobTables(_server.Instance, _oldDb.Name);
                }
                catch (Exception ex)
                {
                    ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                            String.Format("GetBlobTables: URL: {0} Instance {1} Database {2}", url, txtServer.Text, txtName.Text),
                                            ex,
                                            ErrorLog.Severity.Warning);
                }
            }

            // Now we try direct connect
            if (!_sqlServer.OpenConnection(_server.Instance))
                throw new Exception("Unable to open a connection to server.");

            retVal = RawSQL.GetBlobTables(_sqlServer.Connection, _oldDb.Name);
            _sqlServer.CloseConnection();
            return retVal;
        }

        private void ItemActivate_lvBeforeAfterTables(object sender, EventArgs e)
        {
            Click_btnEditBATable(sender, e);
        }

        private void rbDontAuditUserTables_CheckedChanged(object sender, EventArgs e)
        {
            UpdateBeforeAfterAvailability();
        }

        private void linkLblHelpBestPractices_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            HelpAlias.ShowHelp(this, HelpAlias.SSHELP_AuditingBestPractices);
        }
        #endregion

        #region Sensitive Columns

        private void LoadSensitiveColumns()
        {
            if (_server.SqlVersion == 0)
            {
                _lblSCStatus.Text = CoreConstants.Feature_SensitiveColumnNotAvailableVersionUnknown;
                _pnlSensitiveColumns.Visible = false;
                return;
            }
            else if (!SupportsSensitiveColumns())
            {
                _lblSCStatus.Text = CoreConstants.Feature_SensitiveColumnNotAvailableAgent;
                _pnlSensitiveColumns.Visible = false;
                return;
            }
            if (!LoadTables())
            {
                _lblSCStatus.Text = UIConstants.Error_CantLoadTables;
                _pnlSensitiveColumns.Visible = false;
                return;
            }

            //load the Sensitive column info
            _oldSCTables = new Dictionary<string, List<SensitiveColumnTableRecord>>();
            if (_oldDb.AuditSensitiveColumns)
            {
                List<string> missingTables = new List<string>();
                List<SensitiveColumnTableRecord> tables = SensitiveColumnTableRecord.GetAuditedTables(Globals.Repository.Connection, _server.SrvId, _oldDb.DbId);
                string tableName;
                if (tables == null)
                {
                    SelectedIndexChanged_lvSCTables(null, null);
                    return;
                }
                foreach (SensitiveColumnTableRecord table in tables)
                {

                    if (SupportsSchemas())
                    {
                        tableName = table.FullTableName;
                    }
                    else
                        tableName = table.TableName;

                    ListViewItem x = _lvSCTables.Items.Add(tableName);
                    if (table.Type.Equals(UIConstants.SC_Individual))
                    {
                        x.SubItems.Add(table.SelectedColumns ? Form_TableConfigure.GetColumnString(table.Columns) : UIConstants.SC_AllColumns);
                    }
                    else
                    {
                        IList allColumns;
                        allColumns = new ArrayList();
                        Char delimiter = ',';
                        String[] dataSetTables = tableName.Split(delimiter);
                        foreach (string dataSetTable in dataSetTables)
                        {
                            IList columnList = null;
                            columnList = LoadColumns(dataSetTable);
                            foreach (var o in columnList)
                            {
                                allColumns.Add(dataSetTable + "." + o);
                            }
                        }
                        x.SubItems.Add(table.Columns.Length == allColumns.Count ? UIConstants.SC_AllColumns : Form_TableConfigure.GetColumnString(table.Columns));
                    }
                    x.SubItems.Add(table.Type);
                    // Check if the same Key exists in the dictionary or not. 
                    //Add an dictionary item if it does not exist or update when it exists
                    if (_oldSCTables.ContainsKey(tableName))
                    {
                        _oldSCTables[tableName].Add(table);
                    }
                    else
                    {
                        List<SensitiveColumnTableRecord> newtables = new List<SensitiveColumnTableRecord>();
                        newtables.Add(table);
                        _oldSCTables.Add(tableName, newtables);
                    }

                    if (tableName.Contains(","))
                    {
                        Char delimiter = ',';
                        String[] substrings = tableName.Split(delimiter);
                        List<DatabaseObjectRecord> listDataObject = new List<DatabaseObjectRecord>();
                        foreach (var substring in substrings)
                        {
                            if (_tableObjects.ContainsKey(substring))
                            {
                                listDataObject.Add(_tableObjects[substring]);
                            }
                        }
                        x.Tag = listDataObject;
                    }
                    else if (!_tableObjects.ContainsKey(tableName))
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
            SelectedIndexChanged_lvSCTables(null, null);
            if (_lvSCTables.Items.Count == 0)
                _btnConfigureSC.Enabled = false;//SQlCM-5747 v5.6
        }

        private void _btnAddSCTable_Click(object sender, EventArgs e)
        {
            try
            {
                if (!LoadTables())
                {
                    ErrorMessage.Show(this.Text, UIConstants.Error_CantLoadTables, SQLDirect.GetLastError());
                    _btnAddTable.Enabled = false;
                    return;
                }
                List<string> selectedTables = new List<string>();
                List<RawTableObject> tableList = new List<RawTableObject>();

                foreach (ListViewItem tableItem in _lvSCTables.Items)
                {
                    //Type of the current existing item in the list
                    string tableType = tableItem.SubItems[2].Text;

                    if (tableType != UIConstants.SC_Dataset)
                        selectedTables.Add(tableItem.Text);
                }

                foreach (RawTableObject rto in _tableList)
                {
                    tableList.Add(rto);
                }

                Form_TableAdd frm = new Form_TableAdd(tableList, selectedTables, SupportsSchemas(), UIConstants.Table_Column_Usage.SensitiveColumns);

                if (DialogResult.OK == frm.ShowDialog())
                {
                    Dictionary<string, string> currentColumns = new Dictionary<string, string>();
                    Dictionary<string, string> currentColumnType = new Dictionary<string, string>();
                    _lvSCTables.BeginUpdate();

                    foreach (ListViewItem item in _lvSCTables.Items)
                    {
                        currentColumns[item.Text] = item.SubItems[1].Text;
                        currentColumnType[item.Text] = item.SubItems[2].Text;

                        if (item.SubItems[2].Text == UIConstants.SC_Individual)
                        {
                            item.Remove();
                        }

                    }
                    //_lvSCTables.Items.Clear();
                    if (frm.SelectedTables.Count != 0)
                    {

                        foreach (string tableName in frm.SelectedTables)
                        {
                            ListViewItem x = _lvSCTables.Items.Add(tableName);
                            string value = UIConstants.SC_Individual;
                            string found = null;
                            //if (currentColumns.ContainsKey(tableName))
                            if (currentColumnType.TryGetValue(tableName, out found) && found == value)
                            {
                                x.SubItems.Add(currentColumns[tableName]);
                                x.SubItems.Add(UIConstants.SC_Individual);
                            }
                            else
                            {
                                // This is a new table, so add with the default values
                                x.SubItems.Add(UIConstants.BAD_AllColumns);
                                x.SubItems.Add(UIConstants.SC_Individual);
                            }

                            if (_tableObjects.ContainsKey(tableName))
                            {
                                x.Tag = _tableObjects[tableName];
                            }
                            else
                                x.Tag = null;
                        }
                    }
                    _lvSCTables.Focus();

                    if (_lvSCTables.Items.Count > 0)
                    {
                        //_lvSCTables.TopItem.Selected = true;
                        _btnConfigureSC.Enabled = true;//SQlCM-5747 v5.6
                    }
                    _lvSCTables.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Problem adding Sensitive Columns for auditing: " + ex, "Error");
            }
        }
        private void _btnAddSCDataSet_Click(object sender, EventArgs e)
        {
            try
            {
                if (!LoadTables())
                {
                    ErrorMessage.Show(this.Text, UIConstants.Error_CantLoadTables, SQLDirect.GetLastError());
                    _btnAddSCDataSet.Enabled = false;
                    return;
                }
                List<string> selectedTables = new List<string>();
                List<RawTableObject> tableList = new List<RawTableObject>();
                foreach (RawTableObject rto in _tableList)
                {
                    tableList.Add(rto);
                }

                Form_TableAdd frm = new Form_TableAdd(tableList, selectedTables, SupportsSchemas(), UIConstants.Table_Column_Usage.SensitiveColumns);

                if (DialogResult.OK == frm.ShowDialog())
                {
                    _lvSCTables.BeginUpdate();
                    if (frm.SelectedTables.Count != 0)
                    {
                        string[] datasetTables = frm.SelectedTables.ToArray();
                        string tableName = String.Join(",", datasetTables);
                        ListViewItem x = _lvSCTables.Items.Add(tableName);

                        // This is a new table, so add with the default values                    
                        IList columns;

                        // Get Column names to display
                        columns = new ArrayList();
                        Char delimiter = ',';
                        String[] substrings = tableName.Split(delimiter);

                        //Add items to the grid
                        x.SubItems.Add(UIConstants.SC_AllColumns);
                        x.SubItems.Add(UIConstants.SC_Dataset);
                        List<DatabaseObjectRecord> listDataObject = new List<DatabaseObjectRecord>();
                        foreach (var substring in substrings)
                        {
                            if (_tableObjects.ContainsKey(substring))
                            {
                                listDataObject.Add(_tableObjects[substring]);
                            }
                        }
                        if (listDataObject.Count > 1)
                        {
                            x.Tag = listDataObject;
                        }
                        else if (_tableObjects.ContainsKey(tableName))
                        {
                            x.Tag = _tableObjects[tableName];
                        }
                        else
                            x.Tag = null; // Tags creation finished
                    }

                    _lvSCTables.Focus();

                    if (_lvSCTables.Items.Count > 0)
                    {
                        //_lvSCTables.TopItem.Selected = true;
                        _btnConfigureSC.Enabled = true;//SQlCM-5747 v5.6
                    }
                    _lvSCTables.EndUpdate();
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show("Problem adding Sensitive Columns Data-set for auditing: " + ex, "Error");
            }
        }

        private void SelectedIndexChanged_lvSCTables(object sender, EventArgs e)
        {
            if (_lvSCTables.SelectedItems.Count == 0)
            {
                _btnRemoveSCTable.Enabled = false;
                _btnEditSCTable.Enabled = false;
            }
            else
            {
                _btnRemoveSCTable.Enabled = true;
                _btnEditSCTable.Enabled = _lvSCTables.SelectedItems.Count == 1 && _lvSCTables.SelectedItems[0] != null ? true : false;
            }
        }

        private void ItemActivate_lvSCTables(object sender, EventArgs e)
        {
            Click_btnEditSCTable(sender, e);
        }

        private void Click_btnEditSCTable(object sender, EventArgs e)
        {
            ListView.SelectedListViewItemCollection items = _lvSCTables.SelectedItems;

            if (items.Count == 0)
                return;

            string tableName = items[0].SubItems[0].Text;
            string tableType = items[0].SubItems[2].Text;
            RawTableObject tbl = null;

            bool schemas = SupportsSchemas();
            foreach (RawTableObject rto in _tableList)
            {
                if (schemas)
                {
                    if (rto.FullTableName == tableName)
                    {
                        rto.DisplayName = tableName;
                        tbl = rto;
                        break;
                    }
                    else if (
                                tableName.Contains(",")
                                ||
                                tableType == UIConstants.SC_Dataset
                        )
                    {
                        rto.DisplayName = tableName;
                        tbl = rto;
                        break;
                    }
                }
                else
                {
                    if (rto.TableName == tableName)
                    {
                        rto.DisplayName = tableName;
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

            IList columns;
            Dictionary<string, IList> dataSet = new Dictionary<string, IList>();
            if (tableType == CoreConstants.DatasetColumnType)
            {
                columns = new ArrayList();
                Char delimiter = ',';
                String[] substrings = tableName.Split(delimiter);
                foreach (string substring in substrings)
                {
                    IList columnList = null;
                    columnList = LoadColumns(substring);
                    foreach (var o in columnList)
                    {
                        columns.Add(substring+"."+o);
                    }

                    //Add the column names to the dictionary which will be used for Configure Table Form
                    dataSet.Add(substring, columnList);
                }
            }
            else
            {
                columns = LoadColumns(tableName);
            }

            using (Form_TableConfigure frm = new Form_TableConfigure(tbl, 1, columns, selectedColumns, UIConstants.Table_Column_Usage.SensitiveColumns, dataSet))
            {
                if (frm.ShowDialog(this) == DialogResult.OK)
                {
                    string colList = string.Empty;
                    if (frm.AllColumns)
                    {
                        colList = UIConstants.SC_AllColumns;
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

        //SQlCM-5747 v5.6
        private void Click_btnConfigureSC(object sender, EventArgs e)
        {
            Form_ConfigureSCActivity configSCFrm = new Form_ConfigureSCActivity(_oldDb.SrvId, _oldDb.DbId);
            configSCFrm.ShowDialog();
        }

        private void Click_btnRemoveSCTable(object sender, EventArgs e)
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
            if (_lvSCTables.Items.Count == 0)
                _btnConfigureSC.Enabled = false;//SQlCM-5747 v5.6
        }

        private List<SensitiveColumnTableRecord> GetSCTables(SqlTransaction transaction)
        {
            List<SensitiveColumnTableRecord> retVal = new List<SensitiveColumnTableRecord>();
            LoadTables();
            string errorMsg = "";
            int? maxColId = null;

            //Get Max Column Id
            try
            {
                maxColId = SensitiveColumnTableRecord.GetMaxColId(Globals.Repository.Connection, transaction);
            }

            catch (Exception ex)
            {
                errorMsg = ex.Message;
            }

            foreach (ListViewItem item in _lvSCTables.Items)
            {
                maxColId++;
                if (item.Tag.GetType() == typeof(DatabaseObjectRecord))
                {
                    // Existing code
                    DatabaseObjectRecord dor = (DatabaseObjectRecord)item.Tag;
                    // Check and make sure the table still exists for auditing or skip it which will remove it from auditing
                    if (dor != null)
                    {
                        SensitiveColumnTableRecord sctItem = new SensitiveColumnTableRecord();
                        sctItem.SchemaName = dor.SchemaName;
                        sctItem.TableName = dor.TableName;
                        sctItem.ObjectId = dor.Id;

                        // All + Individual
                        if (item.SubItems[1].Text == UIConstants.BAD_AllColumns && item.SubItems[2].Text == UIConstants.SC_Individual)
                        {
                            sctItem.SelectedColumns = false;
                            sctItem.Type = UIConstants.SC_Individual;
                        }

                        // Selective + Individual
                        else if (item.SubItems[1].Text != UIConstants.BAD_AllColumns && item.SubItems[2].Text == UIConstants.SC_Individual)
                        {
                            sctItem.SelectedColumns = true;
                            sctItem.Type = UIConstants.SC_Individual;
                            if (maxColId == null) //if there is no record for the columnId and this would be the first entry in the table
                            { maxColId = 1; }
                            sctItem.ColumnId = maxColId ?? default(int);

                            foreach (string col in Form_TableConfigure.GetColumns(item.SubItems[1].Text))
                            {
                                sctItem.AddColumn(col);
                            }
                        }

                        // Single Dataset Table
                        else
                        {
                            sctItem.SelectedColumns = true;
                            sctItem.Type = UIConstants.SC_Dataset;
                            if (maxColId == null) //if there is no record for the columnId and this would be the first entry in the table
                            { maxColId = 1; }
                            sctItem.ColumnId = maxColId ?? default(int);

                            if (item.SubItems[1].Text.Equals(UIConstants.SC_AllColumns))
                            {
                                IList columns = LoadColumns(dor.FullTableName);
                                var builder = new System.Text.StringBuilder();
                                string columnNames;
                                foreach (RawColumnObject columnObject in columns)
                                {
                                    builder.Append(sctItem.FullTableName + "." + columnObject.ColumnName).Append(",");
                                }
                                builder.Remove(builder.Length - 1, 1);
                                columnNames = builder.ToString();
                                foreach (string col in columnNames.Split(','))
                                {
                                    sctItem.AddColumn(col);
                                }
                            }
                            else
                            {
                                foreach (string col in Form_TableConfigure.GetColumns(item.SubItems[1].Text))
                                {
                                    sctItem.AddColumn(col);
                                }
                            }
                        }

                        retVal.Add(sctItem);
                    }
                }

                else if (item.Tag.GetType() == typeof(List<DatabaseObjectRecord>))
                {
                    // // Mutiple Dataset Tables
                    foreach (DatabaseObjectRecord dor in (IEnumerable<DatabaseObjectRecord>)(item.Tag))
                    {
                        SensitiveColumnTableRecord sctItem = new SensitiveColumnTableRecord();
                        sctItem.SchemaName = dor.SchemaName;
                        sctItem.TableName = dor.TableName;
                        sctItem.ObjectId = dor.Id;
                        sctItem.Type = UIConstants.SC_Dataset;
                        sctItem.SelectedColumns = true;
                        if (maxColId == null) //if there is no record for the columnId and this would be the first entry in the table
                        { maxColId = 1; }
                        sctItem.ColumnId = maxColId ?? default(int);

                        // Get all columns for the current table
                        var builder = new System.Text.StringBuilder();
                        string columnNames = null;
                        IList columns = LoadColumns(dor.FullTableName);
                        foreach (RawColumnObject o in columns)
                        {
                            builder.Append(sctItem.FullTableName+"."+o.ColumnName).Append(",");
                        }
                        builder.Remove(builder.Length - 1, 1);
                        columnNames = builder.ToString();

                        // Insert all columns into an Array
                        string[] allColumns = columnNames.Split(',');

                        //Get current selected columns and insert into an Array
                        string[] slectedColumns = item.SubItems[1].Text.Split(',');

                        // Get common column names
                        string[] result;
                        if (item.SubItems[1].Text.Equals(UIConstants.BAD_AllColumns))
                        {
                            result = allColumns;
                        }
                        else
                        {
                            result = allColumns.Intersect(slectedColumns).ToArray();
                        }

                        // Add the common columns to scItem
                        foreach (string col in result)
                        {
                            sctItem.AddColumn(col);
                        }

                        retVal.Add(sctItem);
                    }
                }
            }
            return retVal;
        }

        #endregion

        #region Privileged User Handling

        //---------------------------------------------------------------------------
        // LoadPrivilegedUsers - loads server roles and users
        //---------------------------------------------------------------------------
        private void
           LoadPrivilegedUsers()
        {
            // SQLCM-5375 Greying logic for privileged users
            UserList serverPrivilegedUsers = new UserList(_server.AuditUsersList);

            lstPrivilegedUsers.BeginUpdate();
            lstPreSelectedPrivilegedUsers.BeginUpdate();

            UserList userList = new UserList(_oldDb.AuditPrivUsersList);

            // Add logins to server level privileged users list
            foreach (Login l in serverPrivilegedUsers.Logins)
            {
                ListViewItem vi = lstPreSelectedPrivilegedUsers.Items.Add(l.Name);
                vi.ToolTipText = "This user has already been set at the Server Level.";
                vi.Tag = l.Sid;
                vi.ImageIndex = (int)AppIcons.Img16.WindowsUser;
                // SQLCM-5375 Greying logic for privileged users
                if (serverPrivilegedUsers.Logins.Any(serverLogin => serverLogin.Name == l.Name))
                {
                    vi.ForeColor = System.Drawing.Color.Gray;
                    vi.UseItemStyleForSubItems = true;
                }
            }

            // Add server roles to server level privileged users list
            foreach (ServerRole r in serverPrivilegedUsers.ServerRoles)
            {
                ListViewItem vi = lstPreSelectedPrivilegedUsers.Items.Add(r.FullName);
                vi.ToolTipText = "This user has already been set at the Server Level.";
                vi.Tag = r.Id;
                vi.ImageIndex = (int)AppIcons.Img16.Role;
                // SQLCM-5375 Greying logic for privileged users
                if (serverPrivilegedUsers.ServerRoles.Any(serverSr => serverSr.CompareName(r)))
                {
                    vi.ForeColor = System.Drawing.Color.Gray;
                    vi.UseItemStyleForSubItems = true;
                }
            }

            // Add logins to database level privileged users list
            foreach (Login l in userList.Logins)
            {
                ListViewItem vi = lstPrivilegedUsers.Items.Add(l.Name);
                vi.Tag = l.Sid;
                vi.ImageIndex = (int)AppIcons.Img16.WindowsUser;
            }

            // Add server roles to database level privileged users list
            foreach (ServerRole r in userList.ServerRoles)
            {
                ListViewItem vi = lstPrivilegedUsers.Items.Add(r.FullName);
                vi.Tag = r.Id;
                vi.ImageIndex = (int)AppIcons.Img16.Role;
            }

            lstPrivilegedUsers.EndUpdate();
            lstPreSelectedPrivilegedUsers.EndUpdate();

            if (lstPrivilegedUsers.Items.Count > 0)
            {
                grpPrivilegedUserActivity.Enabled = Globals.isAdmin;
                lstPrivilegedUsers.TopItem.Selected = Globals.isAdmin;
                btnRemovePriv.Enabled = Globals.isAdmin;
            }
            else
            {
                grpPrivilegedUserActivity.Enabled = false;
                btnRemovePriv.Enabled = false;
            }
        }

        private string GetPrivilegedUserProperty()
        {
            int count = 0;

            UserList ul = new UserList();

            foreach (ListViewItem vi in lstPrivilegedUsers.Items)
            {
                count++;
                if (vi.ImageIndex == (int)AppIcons.Img16.Role)
                {
                    ul.AddServerRole(vi.Text, vi.Text, (int)vi.Tag);
                }
                else
                {
                    ul.AddLogin(vi.Text, (byte[])vi.Tag);
                }
            }

            return (count == 0) ? "" : ul.ToString();
        }

        private void btnAddPriv_Click(object sender, EventArgs e)
        {
            Form_PrivUser frm = new Form_PrivUser(_server.Instance, true, true);
            //frm.MainForm = this.mainForm;                                                      
            if (DialogResult.OK == frm.ShowDialog())
            {
                lstPrivilegedUsers.BeginUpdate();

                lstPrivilegedUsers.SelectedItems.Clear();

                foreach (ListViewItem itm in frm.listSelected.Items)
                {
                    bool found = false;
                    foreach (ListViewItem s in lstPrivilegedUsers.Items)
                    {
                        if (itm.Text == s.Text)
                        {
                            found = true;
                            s.Selected = true;
                            break;
                        }
                    }

                    if (!found)
                    {
                        ListViewItem newItem = new ListViewItem(itm.Text);
                        newItem.Tag = itm.Tag;
                        newItem.ImageIndex = itm.ImageIndex;
                        lstPrivilegedUsers.Items.Add(newItem);
                    }
                }

                lstPrivilegedUsers.EndUpdate();

                grpPrivilegedUserActivity.Enabled = (lstPrivilegedUsers.Items.Count != 0);

                if (lstPrivilegedUsers.Items.Count > 0)
                {
                    grpPrivilegedUserActivity.Enabled = true;
                    lstPrivilegedUsers.TopItem.Selected = true;
                    btnRemovePriv.Enabled = true;
                }
                else
                {
                    grpPrivilegedUserActivity.Enabled = false;
                    btnRemovePriv.Enabled = false;
                }
            }
        }

        private void btnRemovePriv_Click(object sender, EventArgs e)
        {
            if (lstPrivilegedUsers.SelectedItems.Count == 0)
            {
                btnRemovePriv.Enabled = false;
                return;
            }

            lstPrivilegedUsers.BeginUpdate();

            int ndx = lstPrivilegedUsers.SelectedIndices[0];

            foreach (ListViewItem priv in lstPrivilegedUsers.SelectedItems)
            {
                priv.Remove();
            }

            lstPrivilegedUsers.EndUpdate();

            grpPrivilegedUserActivity.Enabled = (lstPrivilegedUsers.Items.Count != 0);

            // reset selected item
            if (lstPrivilegedUsers.Items.Count != 0)
            {
                lstPrivilegedUsers.Focus();
                if (ndx >= lstPrivilegedUsers.Items.Count)
                {
                    lstPrivilegedUsers.Items[lstPrivilegedUsers.Items.Count - 1].Selected = true;
                }
                else
                    lstPrivilegedUsers.Items[ndx].Selected = true;
            }
            else
            {
                grpPrivilegedUserActivity.Enabled = false;
                btnRemovePriv.Enabled = false;
            }
        }

        private void lstPrivilegedUsers_SelectedIndexChanged(object sender, EventArgs e)
        {
            // SQLCM-5375 Greying logic for privileged users
            this.lstPrivilegedUsers.SelectedIndexChanged -= new System.EventHandler(this.lstPrivilegedUsers_SelectedIndexChanged);

            foreach (ListViewItem priv in lstPrivilegedUsers.SelectedItems)
            {
                if (priv.ForeColor == System.Drawing.Color.Gray)
                {
                    priv.Selected = false;
                }
            }
            this.lstPrivilegedUsers.SelectedIndexChanged += new System.EventHandler(this.lstPrivilegedUsers_SelectedIndexChanged);

            if (lstPrivilegedUsers.SelectedItems.Count == 0)
            {
                btnRemovePriv.Enabled = false;
            }
            else
            {
                btnRemovePriv.Enabled = Globals.isAdmin;
            }
        }

        #endregion

        //--------------------------------------------------------------------
        // rbUserSelected_CheckedChanged
        //--------------------------------------------------------------------
            private void rbUserSelected_CheckedChanged(object sender, EventArgs e)
        {
            grpAuditUserActivity.Enabled = !rbAuditUserAll.Checked;

            // SQLCM-5622 Toggling between 'Audited Activity' options enables 'Capture...' options
            if (rbAuditUserSelected.Checked && (chkAuditUserDML.Checked || chkAuditUserSELECT.Checked)
                && CoreConstants.AllowCaptureSql && !_server.AuditUserCaptureSQL && !_chkCaptureSQL.Checked)
                chkUserCaptureSQL.Enabled = Globals.isAdmin;
            else
                chkUserCaptureSQL.Enabled = false;

            // SQLCM-5622 Toggling between 'Audited Activity' options enables 'Capture...' options
            if (rbAuditUserSelected.Checked && (chkAuditUserDDL.Checked || chkAuditUserSecurity.Checked)
                && CoreConstants.AllowCaptureSql && !_server.AuditUserCaptureDDL && !chkDBCaptureDDL.Checked)
                chkUserCaptureDDL.Enabled = Globals.isAdmin;
            else
                chkUserCaptureDDL.Enabled = false;

            // SQLCM-5622 Toggling between 'Audited Activity' options enables 'Capture...' options
            if (rbAuditUserSelected.Checked && (chkAuditUserDML.Checked) && CoreConstants.AllowCaptureSql
                && !_server.AuditUserCaptureTrans && !_chkCaptureTrans.Checked)
            {
                chkUserCaptureSQL.Enabled = Globals.isAdmin;
                chkUserCaptureTrans.Enabled = Globals.isAdmin;
            }
            else
            {
                chkUserCaptureSQL.Enabled = false;
                chkUserCaptureTrans.Enabled = false;
            }
        }

        //--------------------------------------------------------------------
        // chkUserCaptureSQL_CheckedChanged
        //--------------------------------------------------------------------
        private void chkUserCaptureSQL_CheckedChanged(object sender, EventArgs e)
        {
            if (_isLoaded && chkUserCaptureSQL.Checked)
            {
                ErrorMessage.Show(this.Text,
                                  UIConstants.Warning_CaptureAll,
                                  "",
                                  MessageBoxIcon.Warning);
            }
        }

        private void chkExcludes_Click(object sender, EventArgs e)
        {
            if (_cbUserFilterAccessCheck.Checked && _cbUserFilterAccessCheck.Enabled)
            {
                _rbUserAuditPassed.Enabled = true;
                _rbUserAuditFailed.Enabled = true;
            }
            else
            {
                _rbUserAuditPassed.Enabled = false;
                _rbUserAuditFailed.Enabled = false;
            }
        }

        private void chkAuditUserDML_CheckedChanged(object sender, EventArgs e)
        {
            //DML only property
            if (rbAuditUserSelected.Checked && chkAuditUserDML.Checked && ServerRecord.CompareVersions(_server.AgentVersion, "3.5") >= 0)
                chkUserCaptureTrans.Enabled = !this._server.AuditUserCaptureTrans && Globals.isAdmin;  // Handle enabling only considering Server Level - User Privilege property
            else
                chkUserCaptureTrans.Enabled = false;

            //DML or SELECT property
            if (rbAuditUserSelected.Checked && (chkAuditUserDML.Checked || chkAuditUserSELECT.Checked) && CoreConstants.AllowCaptureSql)
                chkUserCaptureSQL.Enabled = !this._server.AuditUserCaptureSQL && Globals.isAdmin;  // Handle enabling only considering Server Level - User Privilege property
            else
                chkUserCaptureSQL.Enabled = false;
        }

        private void chkAuditUserDDL_CheckedChanged(object sender, EventArgs e)
        {
            if (rbAuditUserSelected.Checked && (chkAuditUserDDL.Checked || chkAuditUserSecurity.Checked) && CoreConstants.AllowCaptureSql)
                chkUserCaptureDDL.Enabled = !this._server.AuditUserCaptureDDL && Globals.isAdmin;  // Handle enabling only considering Server Level - User Privilege property
            else
                chkUserCaptureDDL.Enabled = false;
        }

        private void _chkAuditDDL_CheckedChanged(object sender, EventArgs e)
        {
            if (rbAuditUserSelected.Checked && (_chkAuditDDL.Checked || _chkAuditSecurity.Checked) && CoreConstants.AllowCaptureSql)
                chkDBCaptureDDL.Enabled = Globals.isAdmin;
            else
                chkDBCaptureDDL.Enabled = false;
        }

        private void FormDatabasePropertiesLoad(object sender, EventArgs e)
        {
            // Register Checkboxes for deselections
            this.deselectionManager.RegisterCheckbox(this._chkAuditDDL, DeselectControls.DbDatabaseDefinition);
            this.deselectionManager.RegisterCheckbox(this._chkAuditSecurity, DeselectControls.DbSecurityChanges);
            this.deselectionManager.RegisterCheckbox(this._chkAuditAdmin, DeselectControls.DbAdministrativeActivities);
            this.deselectionManager.RegisterCheckbox(this._chkAuditDML, DeselectControls.DbDatabaseModifications);
            this.deselectionManager.RegisterCheckbox(this._chkAuditSELECT, DeselectControls.DbDatabaseSelect);
            this.deselectionManager.RegisterCheckbox(this._chkCaptureSQL, DeselectControls.DbCaptureSqlDmlSelect, this.chkCaptureSQL_CheckedChanged);
            this.deselectionManager.RegisterCheckbox(this._chkCaptureTrans, DeselectControls.DbCaptureSqlTransactionStatus);
            this.deselectionManager.RegisterCheckbox(this.chkDBCaptureDDL, DeselectControls.DbCaptureSqlDdlSecurity);
            this.deselectionManager.RegisterCheckbox(this._cbFilterOnAccess, DeselectControls.DbFilterEvents);
            this.deselectionManager.RegisterRadioButton(this._rbPassed, DeselectControls.DbFilterEventsPassOnly);
            this.deselectionManager.RegisterRadioButton(this._rbFailed, DeselectControls.DbFilterEventsFailedOnly);

            // Update IsLoaded for events restoration
            this._isLoaded = true;
        }

        /// <summary>
        /// Update Ui Controls based on the properties
        /// </summary>
        /// <param name="checkedValue">True if control is selected, false otherwise</param>
        /// <param name="deselectValue">Contains Deselection Options 
        /// 1. Current Level or other and 
        /// 2. Deselection Controls - Parent Control related information
        /// </param>
        public void UpdateUiControls(bool checkedValue, DeselectValues deselectValue)
        {
            var deselectOption = deselectValue.DeselectOption;
            var deselectControl = deselectValue.DeselectControl;
            // perform action on deselect options and property
            switch (deselectControl)
            {
                case DeselectControls.DbDatabaseDefinition:
                    checkedValue = checkedValue || _chkAuditDDL.Checked;
                    this.UpdateDependentCheckboxes(this.chkAuditUserDDL, checkedValue, deselectOption, deselectControl);
                    break;
                case DeselectControls.DbSecurityChanges:
                    checkedValue = checkedValue || _chkAuditSecurity.Checked;
                    this.UpdateDependentCheckboxes(this.chkAuditUserSecurity, checkedValue, deselectOption, deselectControl);
                    break;
                case DeselectControls.DbAdministrativeActivities:
                    checkedValue = checkedValue || _chkAuditAdmin.Checked;
                    this.UpdateDependentCheckboxes(this.chkAuditUserAdmin, checkedValue, deselectOption, deselectControl);
                    break;
                case DeselectControls.DbDatabaseModifications:
                    checkedValue = checkedValue || _chkAuditDML.Checked;
                    this.UpdateDependentCheckboxes(this.chkAuditUserDML, checkedValue, deselectOption, deselectControl);
                    break;
                case DeselectControls.DbDatabaseSelect:
                    checkedValue = checkedValue || _chkAuditSELECT.Checked;
                    this.UpdateDependentCheckboxes(this.chkAuditUserSELECT, checkedValue, deselectOption, deselectControl);
                    break;
                case DeselectControls.DbFilterEvents:
                    this.UpdateDependentCheckboxes(this._cbUserFilterAccessCheck, checkedValue, deselectOption, deselectControl);
                    this.UpdateDependentRadioButtons(this._rbUserAuditPassed, checkedValue, this._rbPassed.Checked, deselectOption, deselectControl);
                    this.UpdateDependentRadioButtons(this._rbUserAuditFailed, checkedValue && this._cbUserFilterAccessCheck.Checked, this._rbFailed.Checked, deselectOption, deselectControl);
                    this.chkExcludes_Click(_cbUserFilterAccessCheck, null);

                    break;
                case DeselectControls.DbFilterEventsPassOnly:
                    this.UpdateDependentRadioButtons(this._rbUserAuditPassed, checkedValue, this._rbPassed.Checked, deselectOption, deselectControl);
                    this.UpdateDependentRadioButtons(this._rbUserAuditFailed, checkedValue, this._rbFailed.Checked, deselectOption, deselectControl);
                    this.chkExcludes_Click(_cbUserFilterAccessCheck, null);

                    break;
                case DeselectControls.DbFilterEventsFailedOnly:
                    this.UpdateDependentRadioButtons(this._rbUserAuditPassed, checkedValue, this._rbPassed.Checked, deselectOption, deselectControl);
                    this.UpdateDependentRadioButtons(this._rbUserAuditFailed, checkedValue, this._rbFailed.Checked, deselectOption, deselectControl);
                    this.chkExcludes_Click(_cbUserFilterAccessCheck, null);

                    break;
                case DeselectControls.DbCaptureSqlDmlSelect:
                    checkedValue = checkedValue || _chkCaptureSQL.Checked;
                    this.UpdateDependentCheckboxes(this.chkUserCaptureSQL, checkedValue, deselectOption, deselectControl, this.chkUserCaptureSQL_CheckedChanged);
                    break;
                case DeselectControls.DbCaptureSqlTransactionStatus:
                    checkedValue = checkedValue || _chkCaptureTrans.Checked;
                    this.UpdateDependentCheckboxes(this.chkUserCaptureTrans, checkedValue, deselectOption, deselectControl);
                    break;
                case DeselectControls.DbCaptureSqlDdlSecurity:
                    // SQLCM-5942 - Deselecting 'Capture DDL' at Audited Level does not deselect at privileged user level
                    checkedValue = checkedValue || chkDBCaptureDDL.Checked;
                    this.UpdateDependentCheckboxes(this.chkUserCaptureDDL, checkedValue, deselectOption, deselectControl);
                    break;
            }
        }

        /// <summary>
        /// Update Dependent control based on deselect options and event handlers
        /// </summary>
        /// <param name="dependentControl"></param>
        /// <param name="checkedValue"></param>
        /// <param name="deselectOptions"></param>
        /// <param name="deselectControl"></param>
        /// <param name="checkedChangedHandler"></param>
        private void UpdateDependentCheckboxes(CheckBox dependentControl, bool checkedValue, DeselectOptions deselectOptions, DeselectControls deselectControl ,EventHandler checkedChangedHandler = null)
        {
            switch (deselectOptions)
            {
                case DeselectOptions.CurrentLevelOnly:
                    dependentControl.Enabled = !checkedValue;
                    break;
                case DeselectOptions.None:
                case DeselectOptions.OtherLevels:
                    dependentControl.Enabled = !checkedValue;
                    var suppressCheckedChangedEventHandler =
                        this.deselectionManager.GetDeselectSuppressCheckBoxChanged(deselectControl);
                    if (checkedChangedHandler != null)
                    {
                        dependentControl.CheckedChanged -= checkedChangedHandler;
                    }
                    if (suppressCheckedChangedEventHandler != null)
                    {
                        dependentControl.CheckedChanged -= suppressCheckedChangedEventHandler;
                    }
                    dependentControl.Checked = checkedValue;
                    if (checkedChangedHandler != null)
                    {
                        dependentControl.CheckedChanged += checkedChangedHandler;
                    }
                    if (suppressCheckedChangedEventHandler != null)
                    {
                        dependentControl.CheckedChanged += suppressCheckedChangedEventHandler;
                    }
                    break;
            }
        }

        /// <summary>
        /// Update Dependent control based on deselect options and event handlers
        /// </summary>
        /// <param name="dependentControl"></param>
        /// <param name="checkedValue"></param>
        /// <param name="radioButtonValue"></param>
        /// <param name="deselectOptions"></param>
        /// <param name="deselectControl"></param>
        /// <param name="checkedChangedHandler"></param>
        private void UpdateDependentRadioButtons(RadioButton dependentControl, bool checkedValue, bool radioButtonValue, DeselectOptions deselectOptions, DeselectControls deselectControl, EventHandler checkedChangedHandler = null)
        {
            switch (deselectOptions)
            {
                case DeselectOptions.CurrentLevelOnly:
                    dependentControl.Enabled = !checkedValue;
                    break;
                case DeselectOptions.None:
                case DeselectOptions.OtherLevels:
                    dependentControl.Enabled = !checkedValue;
                    var suppressCheckedChangedEventHandler =
                        this.deselectionManager.GetDeselectSuppressCheckBoxChanged(deselectControl);
                    if (checkedChangedHandler != null)
                    {
                        dependentControl.CheckedChanged -= checkedChangedHandler;
                    }
                    if (suppressCheckedChangedEventHandler != null)
                    {
                        dependentControl.CheckedChanged -= suppressCheckedChangedEventHandler;
                    }
                    dependentControl.Checked = radioButtonValue;
                    if (checkedChangedHandler != null)
                    {
                        dependentControl.CheckedChanged += checkedChangedHandler;
                    }
                    if (suppressCheckedChangedEventHandler != null)
                    {
                        dependentControl.CheckedChanged += suppressCheckedChangedEventHandler;
                    }
                    break;
            }
        }
    }
}
