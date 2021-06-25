using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Windows.Forms;
using Idera.SQLcompliance.Application.GUI.Helper;
using Idera.SQLcompliance.Application.GUI.Properties;
using Idera.SQLcompliance.Application.GUI.SQL;
using Idera.SQLcompliance.Core;
using Idera.SQLcompliance.Core.Cwf;

namespace Idera.SQLcompliance.Application.GUI.Forms
{
    /// <summary>
    /// Summary description for Form_LoginProperties.
    /// </summary>
    public partial class Form_LoginProperties : Form
    {
        #region Properties

        public RawLoginObject rawLogin;
        private readonly Core.Cwf.LoginAccount _loginAccount;

        #endregion

        #region Constructor / Dispose

        public
           Form_LoginProperties(
              RawLoginObject inRawLogin
           )
        {
            this.StartPosition = FormStartPosition.CenterParent;
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();
            this.Icon = Resources.SQLcompliance_product_ico;


            rawLogin = inRawLogin;

            // web application access
            _loginAccount = new LoginAccount(rawLogin.name);
            chkWebApplicationAccess.Checked = _loginAccount.WebApplicationAccess;


            // reorder the tabs since it's a CF bug
            // by setting the index to "1" it sets it to the top
            // and the existing tabs get "pushed" down
            // this means the tab order on-screen will be in the reverse
            // order that we set here.              |
            tabControl1.Controls.SetChildIndex(this.tabPageAccess, 1);
            tabControl1.Controls.SetChildIndex(this.tabPageGeneral, 1);
        }

        #endregion

        #region OnLoad

        private bool oldGrant;
        private bool oldSysadmin;

        protected override void OnLoad(EventArgs e)
        {
            textName.Text = rawLogin.name;

            radioGrantAccess.Enabled = true;
            radioDenyAccess.Enabled = true;
            //radioThroughGroup.Enabled   = true;

            // load roles
            if (rawLogin.sysadmin == 1)
            {
                radioSysadmin.Checked = true;
            }
            else
            {
                radioAuditor.Checked = true;
            }

            if (rawLogin.denylogin == 1)
            {
                radioDenyAccess.Checked = true;

                radioSysadmin.Enabled = false;
                radioAuditor.Enabled = false;
            }
            else if (rawLogin.hasaccess == 1)
            {
                radioGrantAccess.Checked = true;
            }
            else
            {
                radioThroughGroup.Visible = true;
                radioThroughGroup.Checked = true;
            }

            oldGrant = radioGrantAccess.Checked;
            oldSysadmin = radioSysadmin.Checked;

            // Databases
            LoadDatabases();
            // Reports
            LoadReports();
            base.OnLoad(e);
        }

        #endregion


        #region OK/Apply/Cancel

        //--------------------------------------------------------------------
        // btnCancel_Click - Close without saving
        //--------------------------------------------------------------------
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        //--------------------------------------------------------------------
        // btnOK_Click - Save and close
        //--------------------------------------------------------------------
        private void btnOK_Click(object sender, EventArgs e)
        {
            // validate
            for (int i = 0; i < listDatabases.Items.Count; i++)
            {
                if (IsItemChecked(i))
                {
                    LoginDatabase ldb = (LoginDatabase)(listDatabases.Items[i]);

                    if (!ldb.canViewEvents && !ldb.canViewSQL && !ldb.cannotView)
                    {
                        ErrorMessage.Show(this.Text,
                                          String.Format(UIConstants.Error_NoDatabasePermsSpecified,
                                                         ldb.databaseName));
                        listDatabases.SelectedIndex = i;
                        listDatabases.Focus();
                        return;
                    }
                }
            }
            StringBuilder message = new StringBuilder();
            message.AppendFormat(UIConstants.Message_Permissions, rawLogin.name);

            if (radioGrantAccess.Checked)
            {
                message.Append(UIConstants.Warning_LoginAccess);
            }
            else
            {
                message.Append(UIConstants.Warning_NoLoginAccess);
            }

            if (radioSysadmin.Checked)
            {
                message.Append(UIConstants.Warning_Sysadmin);
            }
            else
            {
                message.Append(UIConstants.Warning_NoSysadmin);
            }

            if (chkWebApplicationAccess.Checked)
                message.Append(UIConstants.Warning_WebAppAccess);
            else
                message.Append(UIConstants.Warning_NoWebAppAccess);

            message.Append("\r\n");
            message.Append(UIConstants.Question_Continue);

            DialogResult choice = MessageBox.Show(message.ToString(),
                                                 UIConstants.Title_ModifyLogin,
                                                 MessageBoxButtons.YesNo,
                                                 MessageBoxIcon.Warning);

            if (choice == DialogResult.No)
                return;

            Cursor = Cursors.WaitCursor;

            if (SaveLogin())
            {
                SaveRoles();
                SaveDatabaseAccess();
                SaveReportAccess();

                // try to grant web app access
                var webAppAccess = chkWebApplicationAccess.Checked;
                SetWebApplicationAccess(_loginAccount, webAppAccess);

                var success = CwfHelper.Instance.SynchronizeUsersWithCwf(_loginAccount);
                if (!success)
                {
                    SetWebApplicationAccess(_loginAccount, !webAppAccess);
                    MessageBox.Show(this,
                                     string.Format("Failed to update Web application access permission for user {0}.", _loginAccount.Name),
                                     UIConstants.AppTitle,
                                     MessageBoxButtons.OK,
                                     MessageBoxIcon.Exclamation);
                }

                string snapshot = GetSnapshot();

                if (snapshot != "")
                {
                    LogRecord.WriteLog(Globals.Repository.Connection,
                                      LogType.ModifyLogin,
                                      Globals.RepositoryServer,
                                      snapshot);
                }

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            Cursor = Cursors.Default;
        }

        private void SetWebApplicationAccess(LoginAccount lAccount, bool webAppAccess)
        {
            lAccount.WebApplicationAccess = webAppAccess;
            lAccount.Set();
        }


        //--------------------------------------------------------------------
        // SaveLogin
        //--------------------------------------------------------------------
        private bool SaveLogin()
        {
            bool retval = true;
            bool grant = false;
            bool deny = false;

            string sql = "";

            // Windows User
            if (radioGrantAccess.Checked && (rawLogin.denylogin == 1 || rawLogin.hasaccess == 0))
            {
                sql = String.Format("exec master..sp_grantlogin {0}",
                                     SQLHelpers.CreateSafeString(rawLogin.name));
                grant = true;
            }
            else if (radioDenyAccess.Checked && rawLogin.denylogin == 0)
            {
                sql = String.Format("exec master..sp_denylogin {0}",
                                     SQLHelpers.CreateSafeString(rawLogin.name));
                deny = true;
            }

            if (sql != "")
            {
                try
                {
                    SqlCommand cmd = new SqlCommand(sql, Globals.Repository.Connection);
                    cmd.CommandTimeout = CoreConstants.sqlcommandTimeout;
                    cmd.ExecuteNonQuery();

                    // Windows User
                    if (grant)
                    {
                        rawLogin.denylogin = 0;
                        rawLogin.hasaccess = 1;
                    }
                    else if (deny)
                    {
                        rawLogin.denylogin = 1;
                        rawLogin.hasaccess = 0;
                    }
                }
                catch (Exception ex)
                {
                    retval = false;
                    ErrorMessage.Show(this.Text, UIConstants.Error_SavingLogin, ex.Message);
                }
            }

            return retval;
        }

        //--------------------------------------------------------------------
        // SaveRoles
        //--------------------------------------------------------------------
        private void SaveRoles()
        {
            // save roles if something has changed
            if (rawLogin.sysadmin == 1 && radioAuditor.Checked)
            {
                // revoke sysadmin role
                SQLRepository.RemoveFromRole(rawLogin.name, "sysadmin");
                rawLogin.sysadmin = 0;
            }
            else if (rawLogin.sysadmin == 0 && radioSysadmin.Checked)
            {
                // add to sysadmin role
                SQLRepository.AddToRole(rawLogin.name, "sysadmin");
                rawLogin.sysadmin = 1;
            }
        }

        //--------------------------------------------------------------------
        // SaveDatabaseAccess
        //--------------------------------------------------------------------
        private void
           SaveDatabaseAccess()
        {
            string db = "";

            try
            {
                bool useWorked = false;
                for (int ndx = 0; ndx < listDatabases.Items.Count; ndx++)
                {
                    LoginDatabase ldb = (LoginDatabase)listDatabases.Items[ndx];
                    db = ldb.databaseName;

                    try
                    {
                        useWorked = false;
                        EventDatabase.UseDatabase(ldb.databaseName, Globals.Repository.Connection);
                        useWorked = true;
                    }
                    catch { } // cant connect to db - skip this one

                    if (!useWorked) continue;

                    if (ldb.chked)
                    {
                        if (!ldb.originalChked)
                        {
                            // add user
                            GrantDbAccess(textName.Text, ldb.databaseName);

                            // grant access
                            if (ldb.canViewEvents)
                            {
                                //The events
                                AddTablePermissions(textName.Text, ldb.databaseName, "Events");
                                AddTablePermissions(textName.Text, ldb.databaseName, "SensitiveColumns");
                                //BAD data
                                AddTablePermissions(textName.Text, ldb.databaseName, "DataChanges");
                                AddTablePermissions(textName.Text, ldb.databaseName, "ColumnChanges");

                                //BAD filters
                                AddTablePermissions(textName.Text, ldb.databaseName, "Columns");
                                AddTablePermissions(textName.Text, ldb.databaseName, "Tables");

                                //event view filters
                                AddTablePermissions(textName.Text, ldb.databaseName, "Applications");
                                AddTablePermissions(textName.Text, ldb.databaseName, "Logins");
                                AddTablePermissions(textName.Text, ldb.databaseName, "Hosts");

                                //Server and DB Summary charts.
                                AddTablePermissions(textName.Text, ldb.databaseName, "Stats");
                            }
                            else
                            {
                                //The events
                                DenyTablePermissions(textName.Text, ldb.databaseName, "Events");
                                DenyTablePermissions(textName.Text, ldb.databaseName, "SensitiveColumns");
                                //BAD data
                                DenyTablePermissions(textName.Text, ldb.databaseName, "DataChanges");
                                DenyTablePermissions(textName.Text, ldb.databaseName, "ColumnChanges");

                                //BAD filters
                                DenyTablePermissions(textName.Text, ldb.databaseName, "Columns");
                                DenyTablePermissions(textName.Text, ldb.databaseName, "Tables");

                                //event view filters
                                DenyTablePermissions(textName.Text, ldb.databaseName, "Applications");
                                DenyTablePermissions(textName.Text, ldb.databaseName, "Logins");
                                DenyTablePermissions(textName.Text, ldb.databaseName, "Hosts");

                                //Server and DB Summary charts.
                                DenyTablePermissions(textName.Text, ldb.databaseName, "Stats");
                            }

                            if (ldb.canViewSQL)
                                AddTablePermissions(textName.Text, ldb.databaseName, "EventSQL");
                            else
                                DenyTablePermissions(textName.Text, ldb.databaseName, "EventSQL");
                        }
                        else
                        {
                            // grant and revoke access appropriately
                            if (ldb.canViewEvents != ldb.originalCanViewEvents)
                            {
                                if (ldb.canViewEvents)
                                {
                                    //The events
                                    AddTablePermissions(textName.Text, ldb.databaseName, "Events");
                                    AddTablePermissions(textName.Text, ldb.databaseName, "SensitiveColumns");
                                    //BAD data
                                    AddTablePermissions(textName.Text, ldb.databaseName, "DataChanges");
                                    AddTablePermissions(textName.Text, ldb.databaseName, "ColumnChanges");

                                    //BAD filters
                                    AddTablePermissions(textName.Text, ldb.databaseName, "Columns");
                                    AddTablePermissions(textName.Text, ldb.databaseName, "Tables");

                                    //event view filters
                                    AddTablePermissions(textName.Text, ldb.databaseName, "Applications");
                                    AddTablePermissions(textName.Text, ldb.databaseName, "Logins");
                                    AddTablePermissions(textName.Text, ldb.databaseName, "Hosts");

                                    //Server and DB Summary charts.
                                    AddTablePermissions(textName.Text, ldb.databaseName, "Stats");
                                }
                                else
                                {
                                    //The events
                                    DenyTablePermissions(textName.Text, ldb.databaseName, "Events");
                                    DenyTablePermissions(textName.Text, ldb.databaseName, "SensitiveColumns");
                                    //BAD data
                                    DenyTablePermissions(textName.Text, ldb.databaseName, "DataChanges");
                                    DenyTablePermissions(textName.Text, ldb.databaseName, "ColumnChanges");

                                    //BAD filters
                                    DenyTablePermissions(textName.Text, ldb.databaseName, "Columns");
                                    DenyTablePermissions(textName.Text, ldb.databaseName, "Tables");

                                    //event view filters
                                    DenyTablePermissions(textName.Text, ldb.databaseName, "Applications");
                                    DenyTablePermissions(textName.Text, ldb.databaseName, "Logins");
                                    DenyTablePermissions(textName.Text, ldb.databaseName, "Hosts");

                                    //Server and DB Summary charts.
                                    DenyTablePermissions(textName.Text, ldb.databaseName, "Stats");
                                }
                            }

                            if (ldb.canViewSQL != ldb.originalCanViewSQL)
                            {
                                if (ldb.canViewSQL)
                                    AddTablePermissions(textName.Text, ldb.databaseName, "EventSQL");
                                else
                                    DenyTablePermissions(textName.Text, ldb.databaseName, "EventSQL");
                            }
                        }
                    }
                    else if (ldb.originalChked)
                    {
                        // revoke user
                        RevokeDbAccess(textName.Text, ldb.databaseName);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorMessage.Show(this.Text,
                                   String.Format("Error setting permissions for {0}", db),
                                   ex.Message);
            }
            finally
            {
                EventDatabase.UseDatabase(CoreConstants.RepositoryDatabase, Globals.Repository.Connection);
            }
        }

        #endregion

        #region Help
        //--------------------------------------------------------------------
        // Form_LoginProperties_HelpRequested - Show Context Sensitive Help
        //--------------------------------------------------------------------
        private void Form_LoginProperties_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            string helpTopic;

            if (tabControl1.SelectedTab == this.tabPageAccess)
                helpTopic = HelpAlias.SSHELP_Form_LoginProperties_Database;
            else
                helpTopic = HelpAlias.SSHELP_Form_LoginProperties_General;

            HelpAlias.ShowHelp(this, helpTopic);

            hlpevent.Handled = true;
        }
        #endregion

        private int currentIndex;
        private bool loading = false;

        private void listDatabases_SelectedIndexChanged(object sender, EventArgs e)
        {
            loading = true;
            listDatabases.BeginUpdate();

            radioDatabaseReadAll.Visible = false;
            radioDatabaseEventsOnly.Visible = false;
            radioDatabaseDeny.Visible = false;

            if (listDatabases.SelectedItems.Count == 0)
            {
                currentIndex = -1;
                groupDatabasePermissions.Text = "Permissions";
            }
            else
            {
                currentIndex = listDatabases.SelectedIndex;
                if (IsItemChecked(currentIndex))
                {
                    radioDatabaseReadAll.Visible = true;
                    radioDatabaseEventsOnly.Visible = true;
                    radioDatabaseDeny.Visible = true;

                    radioDatabaseReadAll.Checked = false;
                    radioDatabaseEventsOnly.Checked = false;
                    radioDatabaseDeny.Checked = false;

                    LoginDatabase lb = (LoginDatabase)(listDatabases.Items[currentIndex]);
                    if (lb.cannotView)
                    {
                        radioDatabaseDeny.Checked = true;
                    }
                    else
                    {
                        if (lb.canViewEvents)
                        {
                            if (lb.canViewSQL)
                                radioDatabaseReadAll.Checked = true;
                            else
                                radioDatabaseEventsOnly.Checked = true;
                        }
                    }
                }

                groupDatabasePermissions.Text = String.Format("Permissions in '{0}'",
                                                               listDatabases.SelectedItems[0].ToString());

                //load checkboxes if the selected item is checked
            }

            listDatabases.EndUpdate();
            loading = false;
        }

        private bool
           IsItemChecked(
              int index
           )
        {
            bool found = false;

            for (int i = 0; i < listDatabases.CheckedIndices.Count; i++)
            {
                if (index == listDatabases.CheckedIndices[i])
                {
                    found = true;
                    break;
                }
            }

            return found;
        }

        private void listDatabases_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            LoginDatabase lb = (LoginDatabase)listDatabases.Items[e.Index];

            if (e.CurrentValue != CheckState.Checked)
            {
                lb.chked = true;
                radioDatabaseReadAll.Visible = true;
                radioDatabaseEventsOnly.Visible = true;
                radioDatabaseDeny.Visible = true;
            }
            else
            {
                lb.chked = false;
                radioDatabaseReadAll.Visible = false;
                radioDatabaseEventsOnly.Visible = false;
                radioDatabaseDeny.Visible = false;
            }
        }

        //-------------------------------------------------------------
        // LoadDatabases
        //--------------------------------------------------------------
        private void LoadDatabases()
        {
            Cursor = Cursors.WaitCursor;

            listDatabases.Items.Clear();

            try
            {
                string cmdstr = String.Format("USE SQLcompliance; SELECT databaseName, instance " +
                                                    "FROM {0} " +
                                                    "WHERE databaseType!='System' " +
                                                    "ORDER by databaseName ASC",
                                              CoreConstants.RepositorySystemDatabaseTable);

                using (SqlCommand cmd = new SqlCommand(cmdstr,
                                                                Globals.Repository.Connection))
                {
                    cmd.CommandTimeout = CoreConstants.sqlcommandTimeout;
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            LoginDatabase logDb = new LoginDatabase();
                            logDb.databaseName = SQLHelpers.GetString(reader, 0);
                            listDatabases.Items.Add(logDb);
                        }
                    }

                    for (int i = 0; i < listDatabases.Items.Count; i++)
                    {
                        LoginDatabase ldb = (LoginDatabase)(listDatabases.Items[i]);

                        if (GetDatabasePermissions(ldb.databaseName,
                                                      out ldb.databaseUser,
                                                      out ldb.canViewEvents,
                                                      out ldb.canViewSQL,
                                                      out ldb.cannotView))
                        {
                            listDatabases.SetItemChecked(i, true);
                            ldb.chked = true;
                            ldb.originalChked = true;

                            // save original values
                            ldb.originalCanViewEvents = ldb.canViewEvents;
                            ldb.originalCanViewSQL = ldb.canViewSQL;
                            ldb.originalCannotView = ldb.cannotView;
                        }
                        else
                        {
                            ldb.chked = false;
                            ldb.originalChked = false;
                            ldb.canViewEvents = true;
                            ldb.canViewSQL = true;
                            ldb.cannotView = false;
                        }
                        listDatabases.Items[i] = ldb;
                    }
                }
            }
            catch (Exception)
            {
            }

            if (listDatabases.Items.Count != 0)
            {
                listDatabases.SelectedIndex = 0;
            }
            Cursor = Cursors.Default;
        }

        private bool
           GetDatabasePermissions(
              string databaseName,
              out string databaseUser,
              out bool canViewEvents,
              out bool canViewSQL,
              out bool cannotView
           )
        {
            bool chked = false;

            databaseUser = "";
            canViewEvents = false;
            canViewSQL = false;
            cannotView = false;

            string hexString = ByteArrayToHex(rawLogin.sid);

            try
            {
                EventDatabase.UseDatabase(databaseName, Globals.Repository.Connection);

                string sql = String.Format("SELECT name FROM {0}..sysusers WHERE sid = {1} AND isaliased = 0",
                                           SQLHelpers.CreateSafeDatabaseName(databaseName),
                                           hexString);
                using (SqlCommand cmd = new SqlCommand(sql, Globals.Repository.Connection))
                {
                    cmd.CommandTimeout = CoreConstants.sqlcommandTimeout;
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            databaseUser = SQLHelpers.GetString(reader, 0);
                            if (databaseUser == null || databaseUser == "")
                            {
                                databaseUser = rawLogin.name;
                            }
                            chked = true;
                        }
                    }
                }

                canViewEvents = CheckTableAccess("Events", rawLogin.name, databaseName);
                canViewSQL = CheckTableAccess("EventSQL", rawLogin.name, databaseName);
                cannotView = !canViewEvents && !canViewSQL;
            }
            catch (Exception)
            {
            }
            finally
            {
                EventDatabase.UseDatabase(CoreConstants.RepositoryDatabase, Globals.Repository.Connection);
            }
            return chked;
        }

        private string
           ByteArrayToHex(
              Byte[] bytes
           )
        {
            StringBuilder sb = new StringBuilder(1024);

            if (bytes.Length != 0)
            {
                sb.Append("0x");
                for (int i = 0; i < bytes.Length; i++)
                {
                    sb.AppendFormat("{0:X2}", bytes[i]);
                }
            }

            return sb.ToString();

        }

        private bool
           CheckTableAccess(
              string table,
              string loginName,
              string databaseName
           )
        {
            bool hasAccess = false;
            string sql;

            try
            {
                sql = String.Format("use {2};exec sp_helprotect {0},{1}",
                                     SQLHelpers.CreateSafeString(table),
                                     SQLHelpers.CreateSafeString(loginName),
                                     SQLHelpers.CreateSafeDatabaseName(databaseName));

                using (SqlCommand cmd = new SqlCommand(sql,
                                                                Globals.Repository.Connection))
                {
                    cmd.CommandTimeout = CoreConstants.sqlcommandTimeout;
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string typ = SQLHelpers.GetString(reader, 4);
                            string action = SQLHelpers.GetString(reader, 5);

                            typ = typ.ToUpper();
                            action = action.ToUpper();

                            if (action.StartsWith("SELECT"))
                            {
                                if (typ.StartsWith("GRANT"))
                                {
                                    hasAccess = true;
                                }
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
            }

            return hasAccess;
        }

        private void
           GrantDbAccess(
              string username,
              string databaseName
           )
        {
            string sql;

            try
            {
                sql = String.Format("use {0};EXEC sp_grantdbaccess {1}",
                                     SQLHelpers.CreateSafeDatabaseName(databaseName),
                                     SQLHelpers.CreateSafeString(username));
                using (SqlCommand cmd = new SqlCommand(sql,
                                                                Globals.Repository.Connection))
                {
                    cmd.CommandTimeout = CoreConstants.sqlcommandTimeout;
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void
           RevokeDbAccess(
              string username,
              string databaseName
           )
        {
            string sql;

            try
            {
                sql = String.Format("use {0};EXEC sp_revokedbaccess {1}",
                                     SQLHelpers.CreateSafeDatabaseName(databaseName),
                                     SQLHelpers.CreateSafeString(username));
                using (SqlCommand cmd = new SqlCommand(sql,
                                                                Globals.Repository.Connection))
                {
                    cmd.CommandTimeout = CoreConstants.sqlcommandTimeout;
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void
           AddTablePermissions(
              string username,
              string databaseName,
              string tableName
           )
        {
            string sql;

            try
            {
                sql = String.Format("use {0};GRANT SELECT ON {1} TO [{2}]",
                                     SQLHelpers.CreateSafeDatabaseName(databaseName),
                                     tableName,
                                     username);
                using (SqlCommand cmd = new SqlCommand(sql, Globals.Repository.Connection))
                {
                    cmd.CommandTimeout = CoreConstants.sqlcommandTimeout;
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void
           DenyTablePermissions(
              string username,
              string databaseName,
              string tableName
           )
        {
            string sql;

            try
            {
                sql = String.Format("use {0};DENY SELECT ON {1} TO [{2}]",
                                     SQLHelpers.CreateSafeDatabaseName(databaseName),
                                     tableName,
                                     username);
                using (SqlCommand cmd = new SqlCommand(sql, Globals.Repository.Connection))
                {
                    cmd.CommandTimeout = CoreConstants.sqlcommandTimeout;
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private string
           GetSnapshot()
        {
            bool dirty = false;
            StringBuilder snapshot = new StringBuilder(1024);

            snapshot.AppendFormat("Login: {0}\r\n\r\n",
                                   textName.Text);

            if (oldGrant != radioGrantAccess.Checked)
            {
                snapshot.AppendFormat("New Security Access:\r\n\t{0}\r\n",
                                     radioDenyAccess.Checked ? "Deny Access"
                                                             : "Grant Access");
                dirty = true;
            }

            if (radioGrantAccess.Checked)
            {
                if (oldSysadmin != radioSysadmin.Checked)
                {
                    snapshot.AppendFormat("New SQL Compliance Manager Permissions:\r\n\t{0}\r\n",
                                         radioSysadmin.Checked ? "Can configure SQL Compliance Manager settings"
                                                               : "Can view and report on audit data");
                    dirty = true;
                }
            }

            if (dirty) snapshot.Append("\r\n");

            // databases

            StringBuilder database = new StringBuilder(2048);
            for (int ndx = 0; ndx < listDatabases.Items.Count; ndx++)
            {
                string db = "";
                LoginDatabase ldb = (LoginDatabase)listDatabases.Items[ndx];

                if (ldb.chked)
                {
                    if (!ldb.originalChked)
                    {
                        db = GetAccess(ldb.canViewEvents, ldb.canViewSQL, ldb.cannotView);

                        db = "Added database user\r\nDefined explicit database access: " + db;
                    }
                    else
                    {
                        string o = GetAccess(ldb.originalCanViewEvents, ldb.originalCanViewSQL, ldb.originalCannotView);
                        string n = GetAccess(ldb.canViewEvents, ldb.canViewSQL, ldb.cannotView);
                        if (o != n)
                        {
                            db = "Access changed:\r\n";
                            db += "\t\tOld: " + o;
                            db += "\r\n";
                            db += "\t\tNew: " + n;
                        }
                    }
                }
                else if (ldb.originalChked)
                {
                    db = "Removed database user and explicit database access";
                }

                if (db != "")
                {
                    database.AppendFormat("Database: {0}\r\n\t{1}\r\n",
                                           ldb.databaseName,
                                           db);
                    dirty = true;
                }
            }

            if (database.Length != 0)
            {
                snapshot.Append(database.ToString());
            }

            if (!dirty)
                return "";
            else
                return snapshot.ToString();
        }

        // TODO:  Double check this
        private string
           GetAccess(
              bool canViewEvents,
              bool canViewSQL,
              bool cannotView
           )
        {
            string db = "";

            if (canViewEvents)
            {
                db += "Can view events";
            }
            if (canViewSQL)
            {
                if (db != "") db += "; ";
                db += "Can view SQL";
            }
            if (cannotView)
            {
                if (db != "") db += "; ";
                db = "Cannot view events or SQL";
            }

            return db;
        }

        private void radioDatabase_CheckedChanged(object sender, EventArgs e)
        {
            if (!loading)
            {
                LoginDatabase lb = (LoginDatabase)(listDatabases.Items[currentIndex]);
                lb.canViewEvents = radioDatabaseReadAll.Checked || radioDatabaseEventsOnly.Checked;
                lb.canViewSQL = radioDatabaseReadAll.Checked;
                lb.cannotView = radioDatabaseDeny.Checked;
            }
        }

        private void listReports_SelectedIndexChanged(object sender, EventArgs e)
        {
            loading = true;
            listReports.BeginUpdate();
            if(listReports.SelectedItems.Count == 0)
            {
                currentIndex = -1;
            }
            else
            {
                currentIndex = listReports.SelectedIndex;
            }

            listReports.EndUpdate();
            loading = false;
        }

        private void LoadReports()
        {
            Cursor = Cursors.WaitCursor;

            listReports.Items.Clear();

            try
            { 
                string cmdstr = String.Format("USE {0}; SELECT ar.[reportname],(select uid from Logins where name = {2}) as uid, " +
                                "(Select [uid] from {1} ra where [uid] = (select uid from Logins where name = {2}) and ar.[rid] = ra.[rid] ) as accesscheck " +
                                "FROM {3} ar",
                                CoreConstants.RepositoryDatabase,
                                CoreConstants.RepositoryReportAccessTable,
                                SQLHelpers.CreateSafeString(rawLogin.name.ToLower()),
                                CoreConstants.RepositoryAuditReportsTable);

                using (SqlCommand cmd = new SqlCommand(cmdstr,Globals.Repository.Connection))
                {
                    cmd.CommandTimeout = CoreConstants.sqlcommandTimeout;
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            LoginReports logReport = new LoginReports();
                            logReport.reportName = SQLHelpers.GetString(reader, 0);
                            logReport.uid = SQLHelpers.GetInt32(reader, 1);
                            logReport.chked = SQLHelpers.GetInt32(reader, 2) > 0 ? false : true;
                            logReport.originalChked = SQLHelpers.GetInt32(reader, 2) > 0 ? false : true;
                            listReports.Items.Add(logReport);
                        }
                    }
                    for (int i = 0; i < listReports.Items.Count; i++)
                    {
                        LoginReports logReport = (LoginReports)(listReports.Items[i]);
                        if (logReport.chked == true)
                        {
                            listReports.SetItemChecked(i, true);
                        }
                        else
                        {
                            listReports.SetItemChecked(i, false);
                        }
                        listReports.Items[i] = logReport;
                    }
                }
            }
            catch (Exception)
            {
            }
            Cursor = Cursors.Default;
        }

        private void listReports_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            LoginReports lb = (LoginReports)listReports.Items[e.Index];

            if (e.CurrentValue != CheckState.Checked)
            {
                lb.chked = true;
            }
            else
            {
                lb.chked = false;
            }
        }

        //--------------------------------------------------------------------
        // SaveDatabaseAccess
        //--------------------------------------------------------------------
        private void
           SaveReportAccess()
        {
            for (int ndx = 0; ndx < listReports.Items.Count; ndx++)
            {
                LoginReports lr = (LoginReports)listReports.Items[ndx];
                if (lr.chked)
                {
                    if (!lr.originalChked)
                    {
                        RawSQL.RevokeReportAccess(lr.reportName, lr.uid, Globals.Repository.Connection);
                    }
                }
                else
                {
                    if (lr.originalChked)
                    {
                        RawSQL.RemoveReportAccess(lr.reportName, lr.uid, Globals.Repository.Connection);
                    }                        
                }
            }
        }
    }
}
