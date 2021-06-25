using Idera.SQLcompliance.Application.GUI.Controls;
using Idera.SQLcompliance.Application.GUI.Helper;
using Idera.SQLcompliance.Application.GUI.Properties;
using Idera.SQLcompliance.Application.GUI.SQL;
using Idera.SQLcompliance.Core;
using Idera.SQLcompliance.Core.Agent;
using Idera.SQLcompliance.Core.Collector;
using Idera.SQLcompliance.Core.Event;
using Idera.SQLcompliance.Core.Remoting;
using Idera.SQLcompliance.Core.Stats;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Idera.SQLcompliance.Application.GUI.Forms
{
    public partial class Form_ApplyServerDefaultAuditSettings : Form
    {
        public Form_ApplyServerDefaultAuditSettings(DefaultSettings defaultSettings)
        {
            InitializeComponent();
            this.Icon = Resources.SQLcompliance_product_ico;
            listView1.View = View.Details;
            ColumnHeader columnHeader = new ColumnHeader();
            columnHeader.Width = listView1.Width;
            listView1.Columns.Add(columnHeader);
            listView1.HeaderStyle = ColumnHeaderStyle.None;

            foreach (ListViewItem item in defaultSettings.GetCheckedServers().Items)
            {
                listView1.Items.Add((ListViewItem)item.Clone());
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            #region
            string selectQuery = String.Format("select * from {0}..{1}", CoreConstants.RepositoryDatabase, CoreConstants.DefaultServerPropertise);
            using (SqlTransaction transaction = Globals.Repository.Connection.BeginTransaction())
            {
                try
                {

                    using (SqlCommand command = new SqlCommand(selectQuery, Globals.Repository.Connection))
                    {
                        command.Transaction = transaction;
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            reader.Read();
                            UserList defaultPrivilegedUserList = new UserList(SQLHelpers.GetString(reader, "auditUsersList"));
                            UserList defaultTrustedUserList = new UserList(SQLHelpers.GetString(reader, "auditTrustedUsersList"));

                            var auditLogins = SQLHelpers.GetBool(reader, "auditLogins");
                            var auditLogouts = SQLHelpers.GetBool(reader, "auditLogouts");
                            var auditFailedLogins = SQLHelpers.GetBool(reader, "auditFailedLogins");
                            var auditDDL = SQLHelpers.GetBool(reader, "auditDDL");
                            var auditSecurity = SQLHelpers.GetBool(reader, "auditSecurity");
                            var auditAdmin = SQLHelpers.GetBool(reader, "auditAdmin");
                            var auditUDE = SQLHelpers.GetBool(reader, "auditUDE");
                            var auditTrace = SQLHelpers.GetBool(reader, "auditTrace");
                            var auditCaptureSQLXE = SQLHelpers.GetBool(reader, "auditCaptureSQLXE");
                            var isAuditLogEnabled = SQLHelpers.GetBool(reader, "isAuditLogEnabled");
                            var auditFailures = SQLHelpers.GetByteToInt(reader, "auditFailures");
                            var auditUserAll = SQLHelpers.GetBool(reader, "auditUserAll");
                            var auditUserLogins = SQLHelpers.GetBool(reader, "auditUserLogins");
                            var auditUserLogouts = SQLHelpers.GetBool(reader, "auditUserLogouts");
                            var auditUserFailedLogins = SQLHelpers.GetBool(reader, "auditUserFailedLogins");
                            var auditUserDDL = SQLHelpers.GetBool(reader, "auditUserDDL");
                            var auditUserSecurity = SQLHelpers.GetBool(reader, "auditUserSecurity");
                            var auditUserAdmin = SQLHelpers.GetBool(reader, "auditUserAdmin");
                            var auditUserDML = SQLHelpers.GetBool(reader, "auditUserDML");
                            var auditUserSELECT = SQLHelpers.GetBool(reader, "auditUserSELECT");
                            var auditUserUDE = SQLHelpers.GetBool(reader, "auditUserUDE");
                            var auditUserFailures = SQLHelpers.GetByteToInt(reader, "auditUserFailures");
                            var auditUserCaptureSQL = SQLHelpers.GetBool(reader, "auditUserCaptureSQL");
                            var auditUserCaptureTrans = SQLHelpers.GetBool(reader, "auditUserCaptureTrans");
                            var auditUserCaptureDDL = SQLHelpers.GetBool(reader, "auditUserCaptureDDL");
                            var defaultAccess = SQLHelpers.GetByteToInt(reader, "defaultAccess");
                            var maxSqlLength = SQLHelpers.GetInt32(reader, "maxSqlLength");

                            #region get the changes to apply at db levels
                            var databaseRecordWithServerDefaultValues = new DatabaseRecord();
                            databaseRecordWithServerDefaultValues.AuditDDL = auditDDL;
                            databaseRecordWithServerDefaultValues.AuditSecurity = auditSecurity;
                            databaseRecordWithServerDefaultValues.AuditAdmin = auditAdmin;
                            databaseRecordWithServerDefaultValues.AuditAccessCheck = (AccessCheckFilter)auditFailures;
                            databaseRecordWithServerDefaultValues.AuditUserAll = auditUserAll;
                            databaseRecordWithServerDefaultValues.AuditUserLogins = auditUserLogins;
                            databaseRecordWithServerDefaultValues.AuditUserLogouts = auditUserLogouts;
                            databaseRecordWithServerDefaultValues.AuditUserFailedLogins = auditUserFailedLogins;
                            databaseRecordWithServerDefaultValues.AuditUserDDL = auditUserDDL;
                            databaseRecordWithServerDefaultValues.AuditUserSecurity = auditUserSecurity;
                            databaseRecordWithServerDefaultValues.AuditUserAdmin = auditUserAdmin;
                            databaseRecordWithServerDefaultValues.AuditUserDML = auditUserDML;
                            databaseRecordWithServerDefaultValues.AuditUserSELECT = auditUserSELECT;
                            databaseRecordWithServerDefaultValues.AuditUserUDE = auditUserUDE;
                            databaseRecordWithServerDefaultValues.AuditUserAccessCheck = (AccessCheckFilter) auditUserFailures;
                            databaseRecordWithServerDefaultValues.AuditUserCaptureSQL = auditUserCaptureSQL;
                            databaseRecordWithServerDefaultValues.AuditUserCaptureTrans = auditUserCaptureTrans;
                            databaseRecordWithServerDefaultValues.AuditUserCaptureDDL = auditUserCaptureDDL;

                            #endregion

                            reader.Close();

                            foreach (ListViewItem item in listView1.Items)
                            {
                                var serverId = Convert.ToInt32(item.Text.Substring(item.Text.LastIndexOf(',') + 1));
                                var serverName = item.Text.Split(',')[0];

                                #region
                                var m_useAgentEnum = false;
                                ICollection roleList = null;
                                ICollection loginList = null;
                                SQLDirect sqlServer = null;
                                try
                                {
                                    ServerRecord sr = new ServerRecord();
                                    sr.Connection = Globals.Repository.Connection;
                                    sr.Read(serverName);
                                    m_useAgentEnum = sr.IsDeployed && sr.IsRunning;
                                }
                                catch
                                {
                                    m_useAgentEnum = false;
                                }
                                if (m_useAgentEnum)
                                {
                                    string url = EndPointUrlBuilder.GetUrl(typeof(AgentManager), Globals.SQLcomplianceConfig.Server, Globals.SQLcomplianceConfig.ServerPort);
                                    try
                                    {
                                        AgentManager manager = GUIRemoteObjectsProvider.AgentManager();


                                        roleList = manager.GetRawServerRoles(serverName);

                                        loginList = manager.GetRawServerLogins(serverName);

                                    }
                                    catch (Exception ex)
                                    {
                                        ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                                                 String.Format("LoadRoles or Logins: URL: {0} Instance {1}", url, serverName),
                                                                 ex,
                                                                 ErrorLog.Severity.Warning);
                                        roleList = null;
                                        loginList = null;
                                        m_useAgentEnum = false;
                                    }
                                }

                                if (roleList == null && loginList == null)
                                {
                                    sqlServer = new SQLDirect();
                                    if (sqlServer.OpenConnection(serverName))
                                    {
                                        roleList = RawSQL.GetServerRoles(sqlServer.Connection);
                                        loginList = RawSQL.GetServerLogins(sqlServer.Connection);
                                    }
                                }

                                if (sqlServer != null)
                                {
                                    sqlServer.CloseConnection();
                                }

                                UserList availablePrivilegedUserList = new UserList();
                                UserList availableTrustedUserList = new UserList();

                                // SQLCM-5849: Handle case-insensitive trusted/privilege user for default settings
                                foreach (var prvLogin in defaultPrivilegedUserList.Logins)
                                {
                                    foreach (var login in loginList)
                                    {
                                        var rawLogin = (RawLoginObject)login;
                                        if (rawLogin.name.Equals(prvLogin.Name, StringComparison.OrdinalIgnoreCase))
                                            availablePrivilegedUserList.AddLogin(rawLogin);
                                    }
                                }

                                foreach (var prvServerRole in defaultPrivilegedUserList.ServerRoles)
                                {
                                    foreach (var role in roleList)
                                    {
                                        var rawRole = (RawRoleObject)role;
                                        // SQLCM-5868: Roles added to default server settings gets added twice at database level
                                        if (prvServerRole.CompareName(rawRole))
                                            availablePrivilegedUserList.AddServerRole(rawRole);
                                    }
                                }

                                foreach (var trustedLogin in defaultTrustedUserList.Logins)
                                {
                                    foreach (var login in loginList)
                                    {
                                        var rawLogin = (RawLoginObject)login;
                                        if (rawLogin.name.Equals(trustedLogin.Name, StringComparison.OrdinalIgnoreCase))
                                            availableTrustedUserList.AddLogin(rawLogin);
                                    }
                                }

                                foreach (var trustedServerRole in defaultTrustedUserList.ServerRoles)
                                {
                                    foreach (var role in roleList)
                                    {
                                        var rawRole = (RawRoleObject)role;
                                        // SQLCM-5868: Roles added to default server settings gets added twice at database level
                                        if (trustedServerRole.CompareName(rawRole))
                                            availableTrustedUserList.AddServerRole(rawRole);
                                    }
                                }

                                var formattedPrvUserList = (availablePrivilegedUserList.Logins.Count() > 0 || availablePrivilegedUserList.ServerRoles.Count() > 0) ?
                                    availablePrivilegedUserList.ToString() : "";
                                var formattedTrustedUserList = (availableTrustedUserList.Logins.Count() > 0 || availableTrustedUserList.ServerRoles.Count() > 0) ?
                                   availableTrustedUserList.ToString() : "";
                                #endregion

                                string updateQuery = string.Format("update {0}..{1}", CoreConstants.RepositoryDatabase, CoreConstants.RepositoryServerTable);
                                updateQuery = updateQuery + " set auditLogins=@auditLogins,auditLogouts=@auditLogouts,auditFailedLogins=@auditFailedLogins," +
                                "auditDDL=@auditDDL,auditSecurity= @auditSecurity, auditAdmin =@auditAdmin," +
                             "auditTrace =@auditTrace,auditUDE = @auditUDE , auditFailures= @auditFailures , auditCaptureSQLXE = @auditCaptureSQLXE," +
                             "isAuditLogEnabled = @isAuditLogEnabled, auditUsersList = @auditUsersList," +
                             "auditUserAll=@auditUserAll , auditUserLogins=@auditUserLogins,auditUserLogouts=@auditUserLogouts,auditUserFailedLogins=@auditUserFailedLogins," +
                             "auditUserDDL= @auditUserDDL , auditUserSecurity=@auditUserSecurity, auditUserAdmin=@auditUserAdmin,auditUserDML = @auditUserDML," +
                             "auditUserSELECT = @auditUserSELECT,auditUserUDE = @auditUserUDE,auditUserFailures=@auditUserFailures,auditUserCaptureSQL=@auditUserCaptureSQL," +
                             "auditUserCaptureTrans=@auditUserCaptureTrans , auditUserCaptureDDL =@auditUserCaptureDDL,defaultAccess=@defaultAccess, maxSqlLength=@maxSqlLength, auditTrustedUsersList=@auditTrustedUsersList";
                                updateQuery = updateQuery + string.Format(" where srvId = {0} and instance = '{1}'", serverId, serverName);

                                using (SqlCommand cmd = new SqlCommand())
                                {
                                    cmd.Transaction = transaction;
                                    cmd.Connection = Globals.Repository.Connection;
                                    cmd.CommandText = updateQuery;

                                    #region
                                    cmd.Parameters.AddWithValue("@auditLogins", auditLogins);
                                    cmd.Parameters.AddWithValue("@auditLogouts", auditLogouts);
                                    cmd.Parameters.AddWithValue("@auditFailedLogins", auditFailedLogins);
                                    cmd.Parameters.AddWithValue("@auditDDL", auditDDL);
                                    cmd.Parameters.AddWithValue("@auditSecurity", auditSecurity);
                                    cmd.Parameters.AddWithValue("@auditAdmin", auditAdmin);
                                    cmd.Parameters.AddWithValue("@auditUDE", auditUDE);
                                    cmd.Parameters.AddWithValue("@auditTrace", auditTrace);
                                    cmd.Parameters.AddWithValue("@auditCaptureSQLXE", auditCaptureSQLXE);
                                    cmd.Parameters.AddWithValue("@isAuditLogEnabled", isAuditLogEnabled);
                                    cmd.Parameters.AddWithValue("@auditFailures", auditFailures);
                                    cmd.Parameters.AddWithValue("@auditUsersList", formattedPrvUserList);
                                    cmd.Parameters.AddWithValue("@auditUserAll", auditUserAll);
                                    cmd.Parameters.AddWithValue("@auditUserLogins", auditUserLogins);
                                    cmd.Parameters.AddWithValue("@auditUserLogouts", auditUserLogouts);
                                    cmd.Parameters.AddWithValue("@auditUserFailedLogins", auditUserFailedLogins);
                                    cmd.Parameters.AddWithValue("@auditUserDDL", auditUserDDL);
                                    cmd.Parameters.AddWithValue("@auditUserSecurity", auditUserSecurity);
                                    cmd.Parameters.AddWithValue("@auditUserAdmin", auditUserAdmin);
                                    cmd.Parameters.AddWithValue("@auditUserDML", auditUserDML);
                                    cmd.Parameters.AddWithValue("@auditUserSELECT", auditUserSELECT);
                                    cmd.Parameters.AddWithValue("@auditUserUDE", auditUserUDE);
                                    cmd.Parameters.AddWithValue("@auditUserFailures", auditUserFailures);
                                    cmd.Parameters.AddWithValue("@auditUserCaptureSQL", auditUserCaptureSQL);
                                    cmd.Parameters.AddWithValue("@auditUserCaptureTrans", auditUserCaptureTrans);
                                    cmd.Parameters.AddWithValue("@auditUserCaptureDDL", auditUserCaptureDDL);
                                    cmd.Parameters.AddWithValue("@defaultAccess", defaultAccess);
                                    cmd.Parameters.AddWithValue("@maxSqlLength", maxSqlLength);
                                    cmd.Parameters.AddWithValue("@auditTrustedUsersList", formattedTrustedUserList);
                                    #endregion

                                    cmd.ExecuteNonQuery();

                                }

                                UpdateThresholds(serverId, transaction);
                                SaveDatabaseRecords(serverId,databaseRecordWithServerDefaultValues, availableTrustedUserList, availablePrivilegedUserList, transaction);
                            }
                        }
                    }

                    transaction.Commit();
                    MessageBox.Show("The settings have been applied to the selected servers.");
                    Close();
                }

                catch (Exception ex)
                {
                    transaction.Rollback();
                    MessageBox.Show("" + ex);
                }
            }
            #endregion
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var auditedActivity = string.Empty;
            var trustedUserLogins = string.Empty;
            var trustedUSerRoles = string.Empty;
            var privUserLogins = string.Empty;
            var privUserRoles = string.Empty;
            var privUserActivity = string.Empty;
            var advanced = string.Empty;
            UserList defaultPrivilegedUserList;
            UserList defaultTrustedUserList;
            var thresholds = GetDefaultThresholds();


            string selectQuery = String.Format("select * from {0}..{1}", CoreConstants.RepositoryDatabase, CoreConstants.DefaultServerPropertise);
            try
            {
                using (SqlCommand command = new SqlCommand(selectQuery, Globals.Repository.Connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        reader.Read();

                        //SQLCM - 5653 v5.6 Create Default Settings_Apply: Clicking 'click here' in apply server settings
                        //and database settings throws invalid cast exception
                        if ((reader[reader.GetOrdinal("auditUsersList")]) != DBNull.Value)
                            defaultPrivilegedUserList = new UserList((String)reader[reader.GetOrdinal("auditUsersList")]);

                        else
                            defaultPrivilegedUserList = new UserList();

                        if (reader[reader.GetOrdinal("auditTrustedUsersList")] != DBNull.Value)
                            defaultTrustedUserList = new UserList((String)reader[reader.GetOrdinal("auditTrustedUsersList")]);
                        else
                            defaultTrustedUserList = new UserList();
                        //

                        foreach (var prvLogin in defaultPrivilegedUserList.Logins)
                        {
                            privUserLogins += ", " + prvLogin.Name;
                        }

                        foreach (var prvServerRole in defaultPrivilegedUserList.ServerRoles)
                        {
                            privUserRoles += ", " + prvServerRole.Name;
                        }

                        foreach (var trustedLogin in defaultTrustedUserList.Logins)
                        {
                            trustedUserLogins += ", " + trustedLogin.Name;
                        }

                        foreach (var trustedServerRole in defaultTrustedUserList.ServerRoles)
                        {
                            trustedUSerRoles += ", " + trustedServerRole.Name;
                        }

                        if (SQLHelpers.GetBool(reader, "auditLogins"))
                            auditedActivity += ", Logins";
                        if (SQLHelpers.GetBool(reader, "auditLogouts"))
                            auditedActivity += ", Logouts";
                        if (SQLHelpers.GetBool(reader, "auditFailedLogins"))
                            auditedActivity += ", Failed Logins";
                        if (SQLHelpers.GetBool(reader, "auditDDL"))
                            auditedActivity += ", Database Definition(DDL)";
                        if (SQLHelpers.GetBool(reader, "auditSecurity"))
                            auditedActivity += ", Security Changes";
                        if (SQLHelpers.GetBool(reader, "auditAdmin"))
                            auditedActivity += ", Administrative Actions";
                        if (SQLHelpers.GetBool(reader, "auditUDE"))
                            auditedActivity += ", User Defined Events";
                        if (SQLHelpers.GetBool(reader, "auditTrace"))
                            auditedActivity += ", Trace Events";
                        if (SQLHelpers.GetBool(reader, "auditCaptureSQLXE"))
                            auditedActivity += ", Extended Events";
                        if (SQLHelpers.GetBool(reader, "isAuditLogEnabled"))
                            auditedActivity += ", SQL Server Audit Specifications";

                        switch (SQLHelpers.GetByteToInt(reader, "auditFailures"))
                        {
                            case 0:
                                auditedActivity += ", Passed";
                                break;
                            case 2:
                                auditedActivity += ", Failed";
                                break;
                        }

                        if (SQLHelpers.GetBool(reader, "auditUserAll"))
                            privUserActivity = ", Audit all activities done by Privileged Users";
                        else
                            privUserActivity = ", Audit selected activities done by Privileged Users";

                        switch (SQLHelpers.GetByteToInt(reader, "defaultAccess"))
                        {
                            case 0:
                                advanced += ", Deny read access by default";
                                break;
                            case 1:
                                advanced += ", Grant right to read events only";
                                break;
                            case 2:
                                advanced += ", Grant right to read events and their associated SQL statements";
                                break;

                        }
                        advanced += ", SQL statement Limit- ";
                        if (SQLHelpers.GetInt32(reader, "maxSqlLength") == -1)
                            advanced += "Store entire text of SQL statements";
                        else
                            advanced += "Truncate stored SQL statements after " + SQLHelpers.GetInt32(reader, "maxSqlLength") + " characters";

                        if (!string.IsNullOrEmpty(auditedActivity))
                            auditedActivity = "Audited Activities: " + auditedActivity.Substring(2) + "!";
                        if (!string.IsNullOrEmpty(trustedUserLogins))
                            trustedUserLogins = "Trusted User Logins: " + trustedUserLogins.Substring(2) + "!";
                        if (!string.IsNullOrEmpty(trustedUSerRoles))
                            trustedUSerRoles = "Trusted User Roles: " + trustedUSerRoles.Substring(2) + "!";
                        if (!string.IsNullOrEmpty(privUserLogins))
                            privUserLogins = "Privileged User Logins: " + privUserLogins.Substring(2) + "!";
                        if (!string.IsNullOrEmpty(privUserRoles))
                            privUserRoles = "Privileged User Roles: " + privUserRoles.Substring(2) + "!";
                        if (!string.IsNullOrEmpty(privUserActivity))
                            privUserActivity = "Privileged User Audit Activity: " + privUserActivity.Substring(2) + "!";
                        if (!string.IsNullOrEmpty(thresholds))
                            thresholds = "Auditing Thresholds: " + thresholds.Substring(2) + "!";
                        if (!string.IsNullOrEmpty(advanced))
                            advanced = "Advanced: " + advanced.Substring(2) + "!";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("" + ex);
            }

            Form_DisplayServerDefaultAuditSettings frm = new Form_DisplayServerDefaultAuditSettings(" servers.", auditedActivity + trustedUserLogins + trustedUSerRoles + privUserLogins + privUserRoles + privUserActivity + thresholds + advanced);
            frm.StartPosition = FormStartPosition.CenterParent;
            frm.ShowDialog(this);
        }

        private string GetDefaultThresholds()
        {
            var selectedThresholds = string.Empty;
            List<ReportCardRecord> records = ReportCardRecord.GetDefaultServerReportCardEntries(Globals.Repository.Connection);

            foreach (var record in records.Where(rcd => rcd.Enabled == true)) //iterate only for set thresholds.
            {
                switch (record.StatisticId)
                {
                    case 4:
                        selectedThresholds += ", Event Alerts";
                        break;
                    case 5:
                        selectedThresholds += ", Privileged User";
                        break;
                    case 6:
                        selectedThresholds += ", Failed Logins";
                        break;
                    case 9:
                        selectedThresholds += ", DDL";
                        break;
                    case 10:
                        selectedThresholds += ", Security";
                        break;
                    case 16:
                        selectedThresholds += ", Logins";
                        break;
                    case 21:
                        selectedThresholds += ", Overall Activity";
                        break;
                    case 23:
                        selectedThresholds += ", Logouts";
                        break;
                }
            }

            return selectedThresholds;
        }

        private void UpdateThresholds(int serverId, SqlTransaction transaction)
        {
            var records = ReportCardRecord.GetDefaultServerReportCardEntries(Globals.Repository.Connection, false, transaction);
            foreach (var record in records)
                record.SrvId = serverId;

            var existingRecords = ReportCardRecord.GetServerReportCardEntries(Globals.Repository.Connection, serverId, transaction);
            var existingStatIds = existingRecords.Select(erecord => erecord.StatisticId);
            var recordsToUpdate = records.Where(record => existingStatIds.Contains(record.StatisticId));
            var recordsToInsert = records.Except(recordsToUpdate);

            foreach (var updateRecord in recordsToUpdate)
                updateRecord.Update(Globals.Repository.Connection, transaction);

            foreach (var insertRecord in recordsToInsert)
                insertRecord.Write(Globals.Repository.Connection, transaction);

        }

        private void SaveDatabaseRecords(int srvId,DatabaseRecord dbWithServerSettings, UserList serTrustedUsers, UserList serPrivilegedUsers, SqlTransaction transaction)
        {
            var dbList = DatabaseRecord.GetDatabases(Globals.Repository.Connection, srvId,transaction);
            if (dbList.Count > 0)
            {
                DatabaseRecord newDb;
                string errorMsg = String.Empty;

                foreach (DatabaseRecord oldDb in dbList)
                {
                    newDb = oldDb.Clone();
                    newDb.Connection = Globals.Repository.Connection;

                    UserList dbTrustedUsers = new UserList(newDb.AuditUsersList); //empty and null case already handled in userlist
                    if (serTrustedUsers.Logins.Length > 0)// if new Trusted users are added. 
                    {
                        foreach (Login l in serTrustedUsers.Logins)
                            dbTrustedUsers.AddLogin(l);

                        newDb.AuditUsersList = dbTrustedUsers.ToString();
                    }
                    if (serTrustedUsers.ServerRoles.Length > 0)
                    {
                        foreach (ServerRole sr in serTrustedUsers.ServerRoles)
                            dbTrustedUsers.AddServerRole(sr);

                        newDb.AuditUsersList = dbTrustedUsers.ToString();
                    }

                    UserList dbPrivUsers = new UserList(newDb.AuditPrivUsersList); //empty and null case already handled in userlist
                    if (serPrivilegedUsers.Logins.Length > 0)// if new Priv users are added. 
                    {
                        foreach (Login l in serPrivilegedUsers.Logins)
                            dbPrivUsers.AddLogin(l);

                        newDb.AuditPrivUsersList = dbPrivUsers.ToString();
                    }
                    if (serPrivilegedUsers.ServerRoles.Length > 0)
                    {
                        foreach (ServerRole sr in serPrivilegedUsers.ServerRoles)
                            dbPrivUsers.AddServerRole(sr);

                        newDb.AuditPrivUsersList = dbPrivUsers.ToString();
                    }

                    newDb.AuditDDL = dbWithServerSettings.AuditDDL || newDb.AuditDDL;
                    newDb.AuditSecurity = dbWithServerSettings.AuditSecurity || newDb.AuditSecurity;
                    newDb.AuditAdmin = dbWithServerSettings.AuditAdmin || newDb.AuditAdmin;

                    //below propertise are not set at server level hence no need to update in db from server
                    //AuditDML,AuditSELECT,AuditCaptureDDL,AuditCaptureTrans,AuditCaptureTrans,AuditCaptureSQL

                    newDb.AuditAccessCheck = dbWithServerSettings.AuditAccessCheck != AccessCheckFilter.NoFilter ?
                        dbWithServerSettings.AuditAccessCheck : newDb.AuditAccessCheck;

                    newDb.AuditUserAll = dbWithServerSettings.AuditUserAll || newDb.AuditUserAll;
                    newDb.AuditUserLogins = dbWithServerSettings.AuditUserLogins || newDb.AuditUserLogins;
                    newDb.AuditUserLogouts = dbWithServerSettings.AuditUserLogouts || newDb.AuditUserLogouts;
                    newDb.AuditUserFailedLogins = dbWithServerSettings.AuditUserFailedLogins || newDb.AuditUserFailedLogins;
                    newDb.AuditUserDDL = dbWithServerSettings.AuditUserDDL || newDb.AuditUserDDL;
                    newDb.AuditUserSecurity = dbWithServerSettings.AuditUserSecurity || newDb.AuditUserSecurity;
                    newDb.AuditUserAdmin = dbWithServerSettings.AuditUserAdmin || newDb.AuditUserAdmin;
                    newDb.AuditUserDML = dbWithServerSettings.AuditUserDML || newDb.AuditUserDML;
                    newDb.AuditUserSELECT = dbWithServerSettings.AuditUserSELECT || newDb.AuditUserSELECT;
                    newDb.AuditUserUDE = dbWithServerSettings.AuditUserUDE || newDb.AuditUserUDE;
                    newDb.AuditUserAccessCheck = dbWithServerSettings.AuditUserAccessCheck != AccessCheckFilter.NoFilter ?
                        dbWithServerSettings.AuditUserAccessCheck : newDb.AuditUserAccessCheck;

                    newDb.AuditUserCaptureSQL = dbWithServerSettings.AuditUserCaptureSQL || newDb.AuditUserCaptureSQL;
                    newDb.AuditUserCaptureTrans = dbWithServerSettings.AuditUserCaptureTrans || newDb.AuditUserCaptureTrans;
                    newDb.AuditUserCaptureDDL = dbWithServerSettings.AuditUserCaptureDDL || newDb.AuditUserCaptureDDL;

                    //SQLCM-5581, 5582
                    //if (_removedTrustedusers.Logins.Length > 0 || _removedTrustedusers.ServerRoles.Length > 0)
                    //{
                    //    foreach (Login l in _removedTrustedusers.Logins)
                    //    {
                    //        users.RemoveLogin(l.Name);
                    //    }
                    //    foreach (ServerRole sr in _removedTrustedusers.ServerRoles)
                    //    {
                    //        users.RemoveServerRole(sr.Name);
                    //    }
                    //    newDb.AuditUsersList = users.ToString();
                    //}


                    try
                    {
                        //---------------------------------------
                        // Write Database Properties if necessary
                        //---------------------------------------
                        if (!DatabaseRecord.Match(newDb, oldDb))
                        {
                            if (!newDb.Write(oldDb, transaction))
                            {
                                errorMsg = DatabaseRecord.GetLastError();
                                throw (new Exception(errorMsg));
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        errorMsg = ex.Message;
                    }

                }
            }
        }
    }
}
