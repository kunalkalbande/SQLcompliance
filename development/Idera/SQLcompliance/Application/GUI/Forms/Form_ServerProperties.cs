using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Idera.SQLcompliance.Application.GUI.Controls;
using Idera.SQLcompliance.Application.GUI.Helper;
using Idera.SQLcompliance.Application.GUI.Properties;
using Idera.SQLcompliance.Application.GUI.SQL;
using Idera.SQLcompliance.Core;
using Idera.SQLcompliance.Core.Agent;
using Idera.SQLcompliance.Core.Collector;
using Idera.SQLcompliance.Core.Event;
using Idera.SQLcompliance.Core.Stats;
using Idera.SQLcompliance.Core.Status;
using System.Net.Sockets;
using System.Data.SqlClient; // v5.6 SQLCM-5373

namespace Idera.SQLcompliance.Application.GUI.Forms
{
    using System.Data.SqlClient;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Text;
    using Core.Templates.AuditTemplates;

    /// <summary>
    /// Summary description for Form_ServerProperties.
    /// </summary>
    /// <remarks>
    /// SQLCM-5375 - 6.1.4.1-Greying Logic and Deselection Implementation
    /// </remarks>
    public partial class Form_ServerProperties : Form, IDeselectionClient
    {
        public enum Context
        {
            General,
            AuditedActivities,
            TrustedUser, // v5.6 SQLCM-5373
            PrivilegedUser,
            Thresholds,
            Advanced
        };

        #region Properties

        public bool ThresholdsDirty = false;
        private bool isDirty = false;
        private bool isLoaded = false;
        private ServerRecord oldServer = null;
        public ServerRecord newServer = null;
        private Dictionary<int, ReportCardRecord> _thresholds;
        //SQLCM-5581, 5582
        private UserList _removedTrustedusers = new UserList();
        private UserList _removedPrivelegedUsers = new UserList();

        // SQLCM-5375 - 6.1.4.1-Greying Logic and Deselection Implementation
        private DeselectionManager deselectionManager;
        #endregion

        #region Constructor / Dispose

        public Form_ServerProperties(
           ServerRecord server
           )
        {
            this.deselectionManager = new DeselectionManager(this);

            this.deselectionManager.ServerRegulations = RegulationSettings.LoadUserAppliedSettingsServer(Globals.Repository.Connection, server.SrvId);
            this.deselectionManager.LoadSettings();

            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();
            this.Icon = Resources.SQLcompliance_product_ico;

            _thresholds = new Dictionary<int, ReportCardRecord>();
            lstTrustedUsers.SmallImageList = AppIcons.AppImageList16(); // v5.6 SQLCM-5373
            lstPrivilegedUsers.SmallImageList = AppIcons.AppImageList16();

            oldServer = server;

            // Tab: General
            txtInstance.Text = server.Instance;
            txtDescription.Text = server.Description;

            if (server.IsAuditedServer)
            {
                string opStatus;
                string auditStatus;

                SQLRepository.GetStatus(server,
                                        out opStatus,
                                        out auditStatus);

                txtOperationStatus.Text = opStatus;
                txtTimeCreated.Text = GetDateString(server.TimeCreated);
                txtTimeLastModified.Text = GetDateString(server.TimeLastModified);
                txtTimeLastHeartbeat.Text = GetDateString(server.TimeLastHeartbeat);

                txtAuditStatus.Text = (server.IsEnabled)
                                         ? UIConstants.Status_Enabled
                                         : UIConstants.Status_Disabled;
                txtTimeLastUpdate.Text = GetDateString(server.LastConfigUpdate);
                txtAuditSettingsStatus.Text = ((server.ConfigVersion > server.LastKnownConfigVersion))
                                                 ? UIConstants.Status_Pending
                                                 : UIConstants.Status_Current;
                btnUpdateAuditSettings.Enabled = (server.ConfigVersion > server.LastKnownConfigVersion);

                txtTimeEventsLastReceived.Text = GetDateString(server.TimeLastCollection);

                if (server.EventDatabase != "")
                    txtEventDatabase.Text = server.EventDatabase;
                else
                    txtEventDatabase.Text = UIConstants.EventDatabaseNotCreated;

                if (!server.ContainsBadEvents)
                    textIntegrityStatus.Text = UIConstants.Info_DatabaseIsClean;
                else
                    textIntegrityStatus.Text = UIConstants.Info_DatabaseHasErrors;

                if (server.TimeLastIntegrityCheck == DateTime.MinValue)
                {
                    textLastIntegrityCheck.Text = UIConstants.Status_Never;
                    textLastIntegrityCheckResults.Text = "";
                }
                else
                {
                    textLastIntegrityCheck.Text = UIUtils.GetLocalTimeDateString(server.TimeLastIntegrityCheck);
                    textLastIntegrityCheckResults.Text = UIUtils.GetIntegrityCheckResult(server.LastIntegrityCheckResult);
                }

                /// maxsql
                if (server.MaxSqlLength < 0)
                {
                    radioUnlimitedSQL.Checked = true;
                    textLimitSQL.Text = "512";
                    textReportLimitSQL.Text = "32763";
                }

                else if (server.MaxSqlLength > 0 && server.MaxSqlLength < 32763)
                {
                    radioLimitSQL.Checked = true;
                    textLimitSQL.Text = server.MaxSqlLength.ToString();
                    textReportLimitSQL.Text = server.MaxSqlLength.ToString();

                }
                else
                {
                    radioLimitSQL.Checked = true;
                    textLimitSQL.Text = server.MaxSqlLength.ToString();
                    textReportLimitSQL.Text = "32763";
                }

                // archive
                if (server.TimeLastArchive == DateTime.MinValue)
                {
                    textTimeLastArchive.Text = UIConstants.Status_Never;
                    textLastArchiveResults.Text = "";
                }
                else
                {
                    textTimeLastArchive.Text = UIUtils.GetLocalTimeDateString(server.TimeLastArchive);
                    textLastArchiveResults.Text = UIUtils.GetArchiveResult(server.LastArchiveResult);
                }
            }
            else
            {
                txtOperationStatus.Text = UIConstants.ServerStatus_NotAudited;
                txtTimeCreated.Text = GetDateString(server.TimeCreated);
                txtTimeLastModified.Text = GetDateString(server.TimeLastModified);
                txtTimeLastHeartbeat.Text = "";

                txtAuditStatus.Text = UIConstants.Status_ReportingOnly;
                txtTimeLastUpdate.Text = "";
                txtAuditSettingsStatus.Text = "";
                btnUpdateAuditSettings.Enabled = false;

                txtTimeEventsLastReceived.Text = "";
                txtEventDatabase.Text = "";

                textIntegrityStatus.Text = "";
            }

            // Set SQL Server Version Field
            // SQL Server 2005,2008
            if (server.SqlVersion < 8 || server.SqlVersion > 15)
            {
                textVersion.Text = "Unknown";
            }
            else
            {
                switch (server.SqlVersion)
                {
                    case 8:
                        textVersion.Text = "2000";
                        break;
                    case 9:
                        textVersion.Text = "2005";
                        break;
                    case 10:
                        textVersion.Text = "2008";
                        break;
                    case 11:
                        textVersion.Text = "2012";
                        break;
                    case 12:
                        textVersion.Text = "2014";
                        break;
                    case 13:
                        textVersion.Text = "2016";
                        break;
                    case 14:
                        textVersion.Text = "2017";
                        break;
                    case 15:
                        textVersion.Text = "2019";
                        break;
                }

                if (server.isClustered)
                    textVersion.Text += " (Virtual)";
            }

            if (server.DefaultAccess == 2)
                radioGrantAll.Checked = true;
            else if (server.DefaultAccess == 1)
                radioGrantEventsOnly.Checked = true;
            else
                radioDeny.Checked = true;

            chkAuditLogins.Checked = false;
            chkAuditLogouts.Checked = false;
            chkAuditFailedLogins.Checked = false;
            chkAuditDDL.Checked = false;
            chkAuditAdmin.Checked = false;
            chkAuditSecurity.Checked = false;
            chkAuditUserDefined.Checked = false;
            _cbFilterAccessCheck.Checked = false;

            // Tab: Audit Settings
            chkAuditLogins.Checked = server.AuditLogins;
            chkAuditLogouts.Checked = server.AuditLogouts;
            chkAuditFailedLogins.Checked = server.AuditFailedLogins;
            chkAuditDDL.Checked = server.AuditDDL;
            chkAuditAdmin.Checked = server.AuditAdmin;
            chkAuditSecurity.Checked = server.AuditSecurity;
            chkAuditUserDefined.Checked = server.AuditUDE;
            if (server.SqlVersion > 10 && !String.IsNullOrEmpty(oldServer.AgentVersion)
                         && float.Parse(oldServer.AgentVersion.Substring(0, 3)) >= 5.4)
            {
                radioXEvents.Enabled = true;
                radioXEvents.Checked = server.AuditCaptureSQLXE;
                if (server.SqlVersion > 13 && float.Parse(oldServer.AgentVersion.Substring(0, 3)) >= 5.5)
                {
                    radioAuditLogs.Enabled = true;
                    radioAuditLogs.Checked = server.IsAuditLogEnabled;
                }
                radioTrace.Enabled = true;
            }
            switch (server.AuditAccessCheck)
            {
                case AccessCheckFilter.FailureOnly:
                    _cbFilterAccessCheck.Checked = true;
                    rbAuditFailedOnly.Checked = true;
                    break;
                case AccessCheckFilter.SuccessOnly:
                    _cbFilterAccessCheck.Checked = true;
                    rbAuditSuccessfulOnly.Checked = true;
                    break;
                case AccessCheckFilter.NoFilter:
                    _cbFilterAccessCheck.Checked = false;
                    rbAuditFailedOnly.Enabled = false;
                    rbAuditSuccessfulOnly.Enabled = false;
                    break;
            }
            //rbAuditSuccessfulOnly.Checked   = ! server.AuditFailures;
            //rbAuditFailedOnly.Checked              = server.AuditFailures;

            // Tab: Audited Users   
            LoadPrivilegedUsers();

            rbAuditUserAll.Checked = server.AuditUserAll;
            rbAuditUserSelected.Checked = !server.AuditUserAll;
            chkAuditUserLogins.Checked = server.AuditUserLogins;
            chkAuditUserLogouts.Checked = server.AuditUserLogouts;
            chkAuditUserFailedLogins.Checked = server.AuditUserFailedLogins;
            chkAuditUserDDL.Checked = server.AuditUserDDL;
            chkAuditUserSecurity.Checked = server.AuditUserSecurity;
            chkAuditUserAdmin.Checked = server.AuditUserAdmin;
            chkAuditUserDML.Checked = server.AuditUserDML;
            chkAuditUserSELECT.Checked = server.AuditUserSELECT;
            chkAuditUserUserDefined.Checked = server.AuditUserUDE;
            switch (server.AuditUserAccessCheck)
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
            chkUserCaptureSQL.Checked = server.AuditUserCaptureSQL;
            chkUserCaptureTrans.Checked = server.AuditUserCaptureTrans;
            chkUserCaptureDDL.Checked = server.AuditUserCaptureDDL;

            //DML only property
            if (rbAuditUserSelected.Checked && chkAuditUserDML.Checked && ServerRecord.CompareVersions(oldServer.AgentVersion, "3.5") >= 0)
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

            //DDL and Security property
            if (rbAuditUserSelected.Checked && (chkAuditUserDDL.Checked || chkAuditUserSecurity.Checked) && CoreConstants.AllowCaptureSql)
                chkUserCaptureDDL.Enabled = true;
            else
            {
                chkUserCaptureDDL.Checked = false;
                chkUserCaptureDDL.Enabled = false;
            }

            grpAuditUserActivity.Enabled = !rbAuditUserAll.Checked;
            //SQLCM -541/4876/5259 v5.6
            chkAddNewDatabases.Checked = server.AddNewDatabasesAutomatically;

            //Load Trusted users v5.6 SQLCM-5373
            LoadTrustedUsers();

            // Report Card Thresholds
            LoadThresholds();

            // reorder the tabs since it's a CF bug
            // by setting the index to "1" it sets it to the top
            // and the existing tabs get "pushed" down
            // this means the tab order on-screen will be in the reverse
            // order that we set here.              |
            if (server.IsAuditedServer)
            {
                tabProperties.Controls.SetChildIndex(this.tabPageAdvanced, 1);
                tabProperties.Controls.SetChildIndex(this.tabPageThresholds, 1);
                tabProperties.Controls.SetChildIndex(this.tabPageUsers, 1);
                tabProperties.Controls.SetChildIndex(this.tabPageAuditSettings, 1);
            }
            tabProperties.Controls.SetChildIndex(this.tabPageGeneral, 1);

            //------------------------------------------------------
            // Make controls read only unless user has admin access
            //------------------------------------------------------
            if (!Globals.isAdmin)
            {
                // handle first tab differently
                txtDescription.Enabled = false;
                btnUpdateAuditSettings.Enabled = false;

                // other tabs
                for (int i = 1; i < tabProperties.TabPages.Count; i++)
                {
                    foreach (Control ctrl in tabProperties.TabPages[i].Controls)
                    {
                        ctrl.Enabled = false;
                    }
                }

                // change buttons
                btnOK.Visible = false;
                btnCancel.Text = "Close";
                btnCancel.Enabled = true;
                this.AcceptButton = btnCancel;
            }

            //------------------------------------------------------
            // Hide some tabs for reporting only server
            //------------------------------------------------------
            if (!server.IsAuditedServer)
            {
                tabProperties.TabPages.Remove(tabPageAuditSettings);
                tabProperties.TabPages.Remove(tabPageUsers);
                tabProperties.TabPages.Remove(tabPageThresholds);
            }

            // make sure we start on general tab
            tabProperties.SelectedTab = tabPageGeneral;
        }

        public void SetContext(Context context)
        {
            switch (context)
            {
                case Context.General:
                    tabProperties.SelectedTab = tabPageGeneral;
                    break;
                case Context.AuditedActivities:
                    tabProperties.SelectedTab = tabPageAuditSettings;
                    break;
                case Context.PrivilegedUser:
                    tabProperties.SelectedTab = tabPageUsers;
                    break;
                case Context.Thresholds:
                    tabProperties.SelectedTab = tabPageThresholds;
                    break;
                case Context.Advanced:
                    tabProperties.SelectedTab = tabPageAdvanced;
                    break;
            }
        }

        //---------------------------------------------------------------------------
        // GetDateString
        //---------------------------------------------------------------------------
        private static string GetDateString(DateTime time)
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

        #region Private Methods

        //-------------------------------------------------------------
        // ValidateProperties
        //-------------------------------------------------------------
        private bool ValidateProperties()
        {
            if (oldServer.IsAuditedServer)
            {
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
                        tabProperties.SelectedTab = tabPageUsers;
                        chkAuditUserLogins.Focus();
                        return false;
                    }
                }

                if (radioUnlimitedSQL.Checked)
                {
                    MessageBox.Show(UIConstants.Warning_ReportLimitSQLLength,
                    UIConstants.Title_RegisteredSQLServerProperties,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                    textLimitSQL.Focus();
                    return true;
                }

                if (radioLimitSQL.Checked)
                {
                    int sqlLimit = UIUtils.TextToInt(textLimitSQL.Text);
                    if (sqlLimit < 1)
                    {
                        ErrorMessage.Show(this.Text,
                                          UIConstants.Error_LimitSQLLength);
                        tabProperties.SelectedTab = this.tabPageAdvanced;
                        textLimitSQL.Focus();
                        return false;
                    }
                    if (sqlLimit > 32763)
                    {
                        MessageBox.Show(UIConstants.Warning_ReportLimitSQLLength,
                        UIConstants.Title_RegisteredSQLServerProperties,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                        textLimitSQL.Focus();
                        return true;
                    }
                }
                if (_checkAlerts.Checked &&
                   !ValidateThreshold(_txtAlertsWarning.Text, _txtAlertsError.Text, "Alerts"))
                    return false;
                if (_checkAllActivity.Checked &&
                   !ValidateThreshold(_txtAllActivityWarning.Text, _txtAllActivityError.Text, "Overall Activity"))
                    return false;
                if (_checkDDL.Checked &&
                   !ValidateThreshold(_txtDDLWarning.Text, _txtDDLError.Text, "DDL"))
                    return false;
                if (_checkFailedLogins.Checked &&
                   !ValidateThreshold(_txtFailedLoginsWarning.Text, _txtFailedLoginsError.Text, "Failed Logins"))
                    return false;
                if (_checkPrivUser.Checked &&
                   !ValidateThreshold(_txtPrivUserWarning.Text, _txtPrivUserError.Text, "Privileged User"))
                    return false;
                if (_checkSecurity.Checked &&
                   !ValidateThreshold(_txtSecurityWarning.Text, _txtSecurityError.Text, "Security"))
                    return false;
                //start sqlcm 5.6 - 5363
                if (_checkLogins.Checked &&
                   !ValidateThreshold(_txtLoginsWarning.Text, _txtLoginsError.Text, "Logins"))
                    return false;
                if (_checkLogouts.Checked &&
               !ValidateThreshold(_txtLogoutsWarning.Text, _txtLogoutsError.Text, "Logouts"))
                    return false;
                //end sqlcm 5.6 - 5363
            }

            return true;
        }

        private bool ValidateThreshold(string warningString, string errorString, string name)
        {
            try
            {
                int warn = Int32.Parse(warningString);
                int error = Int32.Parse(errorString);

                // The real maximum allowed value was ugly, so we have a prettier one.
                if (warn > 2000000000 || error > 2000000000)
                    throw new OverflowException();
                if (warn <= 0 || error <= 0)
                {
                    ErrorMessage.Show(this.Text, String.Format(UIConstants.Error_ThresholdLessThanZero, name));
                    tabProperties.SelectedTab = this.tabPageThresholds;
                    return false;
                }
                if (warn > error)
                {
                    ErrorMessage.Show(this.Text, String.Format(UIConstants.Error_ThresholdErrorLessThanWarn, name));
                    tabProperties.SelectedTab = this.tabPageThresholds;
                    return false;
                }
                return true;
            }
            catch (OverflowException)
            {
                ErrorMessage.Show(this.Text, String.Format(UIConstants.Error_ThresholdOverflow, name));
                tabProperties.SelectedTab = this.tabPageThresholds;
                return false;
            }
            catch (Exception)
            {
                ErrorMessage.Show(this.Text, UIConstants.Error_InvalidThreshold);
                tabProperties.SelectedTab = this.tabPageThresholds;
                return false;
            }
        }

        //-------------------------------------------------------------
        // SaveServerRecord
        //-------------------------------------------------------------
        private bool SaveServerRecord()
        {
            bool retval = false;
            string errorMsg = "";

            isDirty = false;

            if (ValidateProperties())
            {
                newServer = CreateServerRecord();
                newServer.Connection = Globals.Repository.Connection;

                try
                {
                    SaveThresholds();
                    //---------------------------------------
                    // Write Server Properties if necessary
                    //---------------------------------------
                    if (!ServerRecord.Match(newServer, oldServer))
                    {
                        if(!SaveServerLevelUsers(newServer))
                        {
                            errorMsg = ServerRecord.GetLastError();
                            throw (new Exception(errorMsg));
                        }
                        if (!newServer.Write(oldServer))
                        {
                            errorMsg = ServerRecord.GetLastError();
                            throw (new Exception(errorMsg));
                        }
                        else
                        {
                            // update default security
                            if (newServer.DefaultAccess != oldServer.DefaultAccess)
                            {
                                EventDatabase.SetDefaultSecurity(oldServer.EventDatabase,
                                                                 newServer.DefaultAccess,
                                                                 oldServer.DefaultAccess,
                                                                 false,
                                                                 Globals.Repository.Connection);
                            }

                            isDirty = true;
                        }

                        // Handle more complete case where few servers in between are selected to be removed at the server level only
                        // SQLCM-5581, 5582
                        SaveDatabaseRecords(newServer.SrvId); //v5.6 SQLCM - 5373
                    }
                    retval = true;
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
                    if (retval && isDirty && oldServer.IsAuditedServer)
                    {
                        string changeLog = Snapshot.ServerChangeLog(oldServer, newServer);

                        // Register change to server and perform audit log				      
                        ServerUpdate.RegisterChange(newServer.SrvId,
                                                    LogType.ModifyServer,
                                                    newServer.Instance,
                                                    changeLog);

                        // in case agent properties were updated, update all instances
                        //newServer.CopyAgentSettingsToAll(oldServer);

                        newServer.ConfigVersion++;
                    }
                    if (!retval)
                    {
                        ErrorMessage.Show(this.Text,
                                          UIConstants.Error_ErrorSavingServer,
                                          errorMsg);
                    }
                }
            }
            return retval;
        }

        public bool SaveServerLevelUsers(ServerRecord server)
        {
            StringBuilder userSQL = new StringBuilder("");
            userSQL.Append(GetDeleteOldUserForCurrentServerSQL(server));
            userSQL.Append(GetTrustedUsersSQLForServer(server));
            userSQL.Append(GetPrivilegedUsersSQLForServer(server));
            if(server.SaveTrustedandPrivUsers(userSQL))
            {
                return true;
            }
            return false;
        }
        private string GetDeleteOldUserForCurrentServerSQL(ServerRecord serverDetails)
        {
            string deleteSQL = "";
            deleteSQL += serverDetails.GetDeleteSQLForUser(serverDetails.SrvId);
            return deleteSQL;
        }
        private string GetTrustedUsersSQLForServer(ServerRecord serverDetails)
        {
            string trustedUsersSQL = "";
            foreach (ListViewItem vi in lstTrustedUsers.Items)
            {
                if (vi.ImageIndex == (int)AppIcons.Img16.Role)
                {
                    trustedUsersSQL += serverDetails.GetInsertSQLForUser(serverDetails.SrvId, 0, 1, 0, vi.Text, null);
                }
                else
                {
                    trustedUsersSQL += serverDetails.GetInsertSQLForUser(serverDetails.SrvId, 0, 1, 0, null, vi.Text);
                }
            }
            return trustedUsersSQL;
        }
        private string GetPrivilegedUsersSQLForServer(ServerRecord serverDetails)
        {
            string privUsersSQL = "";
            foreach (ListViewItem vi in lstPrivilegedUsers.Items)
            {
                if (vi.ImageIndex == (int)AppIcons.Img16.Role)
                {
                    privUsersSQL += serverDetails.GetInsertSQLForUser(serverDetails.SrvId, 0, 0, 1, vi.Text, null);
                }
                else
                {
                    privUsersSQL += serverDetails.GetInsertSQLForUser(serverDetails.SrvId, 0, 0, 1, null, vi.Text);
                }
            }
            return privUsersSQL;
        }
        //  v5.6 SQLCM-5373
        private bool SaveDatabaseRecords(int srvId)
        {
            List<DatabaseRecord> dbList = ServerRecord.GetDatabasesinSQLInstance(srvId, Globals.Repository.Connection);
            if (dbList.Count > 0)
            {
                SqlTransaction transaction;
                DatabaseRecord newDb;
                string errorMsg = String.Empty;
                foreach (DatabaseRecord oldDb in dbList)
                {
                    newDb = oldDb.Clone();
                    newDb.Connection = Globals.Repository.Connection;
					newDb.AuditUserAll = rbAuditUserAll.Checked; // SQLCM-5922
                    //SQLCM-5581, 5582
                    UserList currentAddedUsers = new UserList();
                    currentAddedUsers.LoadFromString(GetTrustedUserProperty());
                    if (newDb.AuditUsersList != null && newDb.AuditUsersList.Length > 0) // If already existing users are present at Database level.
                    {
                        UserList users = new UserList();
                        users.LoadFromString(newDb.AuditUsersList);
                        bool bfound = false;
                        int userLoginCount = users.Logins.Length;
                        int userServerRoleCount = users.ServerRoles.Length;
                        if (currentAddedUsers.Logins.Length > 0 )// if new Trusted users are added. 
                        {
                            foreach (Login l in currentAddedUsers.Logins)
                            {
                                if (userLoginCount > 0)
                                {
                                    bfound = false;
                                    foreach (Login l1 in users.Logins)
                                    {
                                        if (l.Name.Equals(l1.Name))
                                        {
                                            bfound = true;
                                            break;
                                        }
                                    }
                                    if (!bfound)
                                        users.AddLogin(l);
                                }
                                else
                                {
                                    users.AddLogin(l);
                                }

                            }
                            newDb.AuditUsersList = users.ToString();
                        }
                        if (currentAddedUsers.ServerRoles.Length > 0)
                        {
                            foreach (ServerRole sr in currentAddedUsers.ServerRoles)
                            {
                                if (userServerRoleCount > 0)
                                {
                                    bfound = false;
                                    foreach (ServerRole l1 in users.ServerRoles)
                                    {
                                        // SQLCM-5868: Roles added to default server settings gets added twice at database level
                                        if (sr.CompareName(l1))
                                        {
                                            bfound = true;
                                            break;
                                        }
                                    }
                                    if (!bfound)
                                        users.AddServerRole(sr);
                                }
                                else
                                {
                                    users.AddServerRole(sr);
                                }
                            }
                            newDb.AuditUsersList = users.ToString();
                        }
                        //SQLCM-5581, 5582
                        if (_removedTrustedusers.Logins.Length > 0 || _removedTrustedusers.ServerRoles.Length > 0)
                        {
                            foreach (Login l in _removedTrustedusers.Logins)
                            {
                                users.RemoveLogin(l.Name);
                            }
                            foreach (ServerRole sr in _removedTrustedusers.ServerRoles)
                            {
                                users.RemoveServerRole(sr.Name);
                            }
                            newDb.AuditUsersList = users.ToString();
                        }
                    }
                    else
                    {
                        newDb.AuditUsersList = GetTrustedUserProperty();
                    }

                    using (transaction = Globals.Repository.Connection.BeginTransaction())
                    {
                        try
                        {
                            if (!newDb.Update(oldDb, transaction))
                            {
                                errorMsg = DatabaseRecord.GetLastError();
                            }
                        }
                        catch (Exception ex)
                        {
                            errorMsg = ex.Message;
                        }
                        finally
                        {
                            transaction.Commit();
                        }
                    }

                }
            }
            return false;
        }

        //--------------------------------------------------------------------
        // CreateServerRecord
        //--------------------------------------------------------------------
        private ServerRecord
           CreateServerRecord()
        {
            ServerRecord srv = oldServer.Clone();

            // General
            srv.Description = txtDescription.Text;

            // default access
            if (radioGrantAll.Checked)
                srv.DefaultAccess = 2;
            else if (radioGrantEventsOnly.Checked)
                srv.DefaultAccess = 1;
            else
                srv.DefaultAccess = 0;

            if (srv.IsAuditedServer)
            {
                // Audit Settings		
                srv.AuditLogins = chkAuditLogins.Checked;
                srv.AuditLogouts = chkAuditLogouts.Checked;
                srv.AuditFailedLogins = chkAuditFailedLogins.Checked;
                srv.AuditDDL = chkAuditDDL.Checked;
                srv.AuditAdmin = chkAuditAdmin.Checked;
                srv.AuditSecurity = chkAuditSecurity.Checked;
                srv.AuditUDE = chkAuditUserDefined.Checked;
                if (_cbFilterAccessCheck.Checked)
                {
                    if (rbAuditFailedOnly.Checked)
                        srv.AuditAccessCheck = AccessCheckFilter.FailureOnly;
                    else
                        srv.AuditAccessCheck = AccessCheckFilter.SuccessOnly;
                }
                else
                {
                    srv.AuditAccessCheck = AccessCheckFilter.NoFilter;
                }

                //Trusted users  v5.6 SQLCM-5373
                srv.AuditTrustedUsersList = GetTrustedUserProperty();

                // Privileged users             
                srv.AuditUsersList = GetPrivilegedUserProperty();
                srv.AuditUserAll = rbAuditUserAll.Checked;
                srv.AuditUserLogins = chkAuditUserLogins.Checked;
                srv.AuditUserLogouts = chkAuditUserLogouts.Checked;
                srv.AuditUserFailedLogins = chkAuditUserFailedLogins.Checked;
                srv.AuditUserDDL = chkAuditUserDDL.Checked;
                srv.AuditUserSecurity = chkAuditUserSecurity.Checked;
                srv.AuditUserAdmin = chkAuditUserAdmin.Checked;
                srv.AuditUserDML = chkAuditUserDML.Checked;
                srv.AuditUserSELECT = chkAuditUserSELECT.Checked;
                srv.AuditUserUDE = chkAuditUserUserDefined.Checked;
                if (_cbUserFilterAccessCheck.Checked)
                {
                    if (_rbUserAuditFailed.Checked)
                        srv.AuditUserAccessCheck = AccessCheckFilter.FailureOnly;
                    else
                        srv.AuditUserAccessCheck = AccessCheckFilter.SuccessOnly;
                }
                else
                {
                    srv.AuditUserAccessCheck = AccessCheckFilter.NoFilter;
                }
                srv.AuditUserCaptureSQL = chkUserCaptureSQL.Checked;
                srv.AuditUserCaptureTrans = chkUserCaptureTrans.Checked;
                srv.AuditUserCaptureDDL = chkUserCaptureDDL.Checked;

                if (radioLimitSQL.Checked)
                {
                    srv.MaxSqlLength = UIUtils.TextToInt(textLimitSQL.Text);
                }
                else
                {
                    srv.MaxSqlLength = -1;
                }

                srv.AuditCaptureSQLXE = radioXEvents.Checked;
                srv.IsAuditLogEnabled = radioAuditLogs.Checked;
                //SQLCM -541/4876/5259 v5.6
                srv.AddNewDatabasesAutomatically = chkAddNewDatabases.Checked;
            }

            return srv;
        }

        #endregion

        #region OK / Cancel / Apply

        //-------------------------------------------------------------
        // btnOK_Click
        //-------------------------------------------------------------
        private void btnOK_Click(object sender, EventArgs e)
        {
            // SQLCM-5375 - 6.1.4.1-Greying Logic and Deselection Implementation Save Database Records
            if (SaveServerRecord() && SaveDatabaseRecord())
            {
                if (isDirty)
                    this.DialogResult = DialogResult.OK;
                else
                    this.DialogResult = DialogResult.Cancel;

                this.Close();
            }
        }

        //--------------------------------------------------------------------
        // SaveDatabaseRecord
        //--------------------------------------------------------------------
        private bool SaveDatabaseRecord()
        {
            // Get the Databases
            var dbs = DatabaseRecord.GetDatabases(Globals.Repository.Connection, this.newServer.SrvId);
            if (dbs == null || dbs.Count == 0)
            {
                return true;
            }

            foreach (DatabaseRecord oldDb in dbs)
            {
                var newDb = oldDb.Clone();
                newDb.Connection = Globals.Repository.Connection;

                //SQLCM 5.7 Requirement 5.3.4.2
                IFormatter formatter;
                MemoryStream streamTrustedUsers = null;
                streamTrustedUsers = new MemoryStream(Convert.FromBase64String(newDb.AuditUsersList));
                formatter = new BinaryFormatter();
                RemoteUserList trustedUserList = new RemoteUserList();
                RemoteUserList privilegedUserList = new RemoteUserList();
                if (newDb.AuditUsersList != null && newDb.AuditUsersList != "")
                {
                    trustedUserList = (RemoteUserList)formatter.Deserialize(streamTrustedUsers);
                }
                var error = string.Empty;
                try
                {
                    if (!newDb.SaveDatabaseLevelUsersFromServerSettings(newDb, trustedUserList, privilegedUserList))
                    {
                        error = DatabaseRecord.GetLastError();
                        throw (new Exception(error));
                    }
                }
                catch (Exception ex)
                {
                    error = ex.Message;
                }

                // Execute Update SQL in a transaction
                using (SqlTransaction transaction = Globals.Repository.Connection.BeginTransaction())
                {
                    var errorMsg = string.Empty;
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
                    finally
                    {
                        //-----------------------------------------------------------
                        // Cleanup - Close transaction, update server, display error
                        //-----------------------------------------------------------
                        transaction.Commit();

                        string changeLog = Snapshot.DatabaseChangeLog(
                            Globals.Repository.Connection,
                            oldDb,
                            newDb,
                            string.Empty,
                            string.Empty,
                            string.Empty);

                        // Register change to server and perform audit log				      
                        ServerUpdate.RegisterChange(oldDb.SrvId, LogType.ModifyDatabase, oldDb.SrvInstance, changeLog);
                        if (!string.IsNullOrEmpty(errorMsg))
                        {
                            ErrorMessage.Show(this.Text,
                           UIConstants.Error_ErrorSavingDatabase,
                           errorMsg);
                        }
                    }
                }
            }
            return true;
        }

        //-------------------------------------------------------------
        // btnCancel_Click
        //-------------------------------------------------------------
        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            this.Close();
        }

        #endregion

        #region Form Event Handlers

        //-------------------------------------------------------------
        // btnUpdateAuditSettings_Click
        //-------------------------------------------------------------
        private void btnUpdateAuditSettings_Click(object sender, EventArgs e)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;
                AgentManager manager = GUIRemoteObjectsProvider.AgentManager();
                manager.UpdateAuditConfiguration(oldServer.Instance);

                txtAuditSettingsStatus.Text = UIConstants.Status_Requested;
            }
            catch (Exception ex)
            {
                ErrorMessage.Show(this.Text,
                                  UIConstants.Error_UpdateNowFailed,
                                  UIUtils.TranslateRemotingException(Globals.SQLcomplianceConfig.Server,
                                                                     UIConstants.CollectionServiceName,
                                                                     ex),
                                  MessageBoxIcon.Error);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        //--------------------------------------------------------------------
        // chkUserCaptureSQL_CheckedChanged
        //--------------------------------------------------------------------
        private void chkUserCaptureSQL_CheckedChanged(object sender, EventArgs e)
        {
            if (isLoaded && chkUserCaptureSQL.Checked)
            {
                ErrorMessage.Show(this.Text,
                                  UIConstants.Warning_CaptureAll,
                                  "",
                                  MessageBoxIcon.Warning);
            }
        }

        //--------------------------------------------------------------------
        // rbUserSelected_CheckedChanged
        //--------------------------------------------------------------------
        private void rbUserSelected_CheckedChanged(object sender, EventArgs e)
        {
            grpAuditUserActivity.Enabled = !rbAuditUserAll.Checked;

            if (rbAuditUserSelected.Checked && (chkAuditUserDML.Checked || chkAuditUserSELECT.Checked) && CoreConstants.AllowCaptureSql)
                chkUserCaptureSQL.Enabled = Globals.isAdmin;
            else
                chkUserCaptureSQL.Enabled = false;


            if (rbAuditUserSelected.Checked && (chkAuditUserDDL.Checked || chkAuditUserSecurity.Checked) && CoreConstants.AllowCaptureSql)
                chkUserCaptureDDL.Enabled = Globals.isAdmin;
            else
                chkUserCaptureDDL.Enabled = false;
            if (rbAuditUserSelected.Checked && (chkAuditUserDML.Checked) && CoreConstants.AllowCaptureSql)
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

        #endregion

        #region Help

        //--------------------------------------------------------------------
        // Form_ServerProperties_HelpRequested - Show Context Sensitive Help
        //--------------------------------------------------------------------
        private void Form_ServerProperties_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            string helpTopic;

            if (tabProperties.SelectedTab == tabPageAdvanced)
                helpTopic = HelpAlias.SSHELP_Form_ServerProperties_Advanced;
            else if (tabProperties.SelectedTab == tabPageUsers)
                helpTopic = HelpAlias.SSHELP_Form_ServerProperties_PrivUsers;
            else if (tabProperties.SelectedTab == tabPageAuditSettings)
                helpTopic = HelpAlias.SSHELP_Form_ServerProperties_Activities;
            else if (tabProperties.SelectedTab == this.tabPageThresholds)
                helpTopic = HelpAlias.SSHELP_Form_ServerProperties_Thresholds;
            else
                helpTopic = HelpAlias.SSHELP_Form_ServerProperties_General;

            HelpAlias.ShowHelp(this, helpTopic);
            hlpevent.Handled = true;
        }

        #endregion

        private void chkAuditUserDML_CheckedChanged(object sender, EventArgs e)
        {
            //DML only property
            if (rbAuditUserSelected.Checked && chkAuditUserDML.Checked && ServerRecord.CompareVersions(oldServer.AgentVersion, "3.5") >= 0)
                chkUserCaptureTrans.Enabled = Globals.isAdmin;
            else
                chkUserCaptureTrans.Enabled = false;

            //DML or SELECT property
            if (rbAuditUserSelected.Checked && (chkAuditUserDML.Checked || chkAuditUserSELECT.Checked) && CoreConstants.AllowCaptureSql)
                chkUserCaptureSQL.Enabled = Globals.isAdmin;
            else
                chkUserCaptureSQL.Enabled = false;
        }

        #region Privileged User Handling      

        //---------------------------------------------------------------------------
        // LoadPrivilegedUsers - loads server roles and users
        //---------------------------------------------------------------------------
        private void
           LoadPrivilegedUsers()
        {
            lstPrivilegedUsers.BeginUpdate();

            UserList userList = new UserList(oldServer.AuditUsersList);

            // Add logins
            foreach (Login l in userList.Logins)
            {
                ListViewItem vi = lstPrivilegedUsers.Items.Add(l.Name);
                vi.Tag = l.Sid;
                vi.ImageIndex = (int)AppIcons.Img16.WindowsUser;
            }

            // Add server roles
            foreach (ServerRole r in userList.ServerRoles)
            {
                ListViewItem vi = lstPrivilegedUsers.Items.Add(r.FullName);
                vi.Tag = r.Id;
                vi.ImageIndex = (int)AppIcons.Img16.Role;
            }

            lstPrivilegedUsers.EndUpdate();

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
            Form_PrivUser frm = new Form_PrivUser(oldServer.Instance, true);
            //frm.MainForm = this.mainForm;                                                      
            if (DialogResult.OK == frm.ShowDialog())
            {
                lstPrivilegedUsers.BeginUpdate();

                lstPrivilegedUsers.SelectedItems.Clear();

                foreach (ListViewItem itm in frm.listSelected.Items)
                {
                    bool found = false;
                    // SQLCM-5676: While removing PRIVILEGED USER on server level, pop - up is expected
                    // On adding - Update Privileged Users marked for removal
                    _removedPrivelegedUsers.RemoveLogin(itm.Text);
                    _removedPrivelegedUsers.RemoveServerRole(itm.Text);

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

        private void Form_ServerProperties_Load(object sender, EventArgs e)
        {
            if (lstPrivilegedUsers.Items.Count > 0)
            {
                btnRemovePriv.Enabled = Globals.isAdmin;
                lstPrivilegedUsers.TopItem.Selected = Globals.isAdmin;
            }
            else
            {
                btnRemovePriv.Enabled = false;
            }

            this.isLoaded = true;
        }

        private void auditSettings_CheckedChanged(object sender, EventArgs e)
        {
            var deselectValue = new DeselectValues();
            if (((CheckBox)sender).Name == "chkAuditLogins")
                deselectValue = new DeselectValues(DeselectControls.ServerLogins, DeselectOptions.CurrentLevelOnly, null);
            else if (((CheckBox)sender).Name == "chkAuditLogouts")
                deselectValue = new DeselectValues(DeselectControls.ServerLogouts, DeselectOptions.CurrentLevelOnly, null);
            else if (((CheckBox)sender).Name == "chkAuditFailedLogins")
                deselectValue = new DeselectValues(DeselectControls.ServerFailedLogins, DeselectOptions.CurrentLevelOnly, null);
            else if (((CheckBox)sender).Name == "chkAuditSecurity")
                deselectValue = new DeselectValues(DeselectControls.ServerSecurityChanges, DeselectOptions.CurrentLevelOnly, null);
            else if (((CheckBox)sender).Name == "chkAuditDDL")
                deselectValue = new DeselectValues(DeselectControls.ServerDatabaseDefinition, DeselectOptions.CurrentLevelOnly, null);
            else if (((CheckBox)sender).Name == "chkAuditAdmin")
                deselectValue = new DeselectValues(DeselectControls.ServerAdministrativeActivities, DeselectOptions.CurrentLevelOnly, null);
            else if (((CheckBox)sender).Name == "chkAuditUserDefined")
                deselectValue = new DeselectValues(DeselectControls.ServerUserDefined, DeselectOptions.CurrentLevelOnly, null);
            else if (((CheckBox)sender).Name == "_cbFilterAccessCheck")
                deselectValue = new DeselectValues(DeselectControls.ServerFilterEvents, DeselectOptions.CurrentLevelOnly, null);

            if (((CheckBox)sender).CheckState == CheckState.Checked)
                this.UpdateUiControls(true, deselectValue);
            else
                this.UpdateUiControls(false, deselectValue);
        }

        private void radioUnlimitedSQL_CheckedChanged(object sender, EventArgs e)
        {
            textLimitSQL.Enabled = radioLimitSQL.Checked;
        }

        private void Click_cbFilterAccessCheck(object sender, EventArgs e)
        {
            var deselectValue = new DeselectValues(DeselectControls.ServerFilterEvents, DeselectOptions.CurrentLevelOnly, null);
            if (_cbFilterAccessCheck.Checked)
            {
                rbAuditFailedOnly.Enabled = true;
                rbAuditSuccessfulOnly.Enabled = true;
                this.UpdateUiControls(true, deselectValue);
            }
            else
            {
                rbAuditFailedOnly.Enabled = false;
                rbAuditSuccessfulOnly.Enabled = false;
                this.UpdateUiControls(false, deselectValue);
            }
        }

        private void chkExcludes_Click(object sender, EventArgs e)
        {
            if (_cbUserFilterAccessCheck.Checked)
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

        /// <summary>
        /// Load the thersholds for this server from the database.  If thresholds are not present
        /// for certain statistics, these stats are set to -1.
        /// </summary>
        private void LoadThresholds()
        {
            try
            {
                List<ReportCardRecord> records =
                   ReportCardRecord.GetServerReportCardEntries(Globals.Repository.Connection, oldServer.SrvId);
                ReportCardRecord current;
                foreach (ReportCardRecord record in records)
                    _thresholds.Add(record.StatisticId, record);

                current = _thresholds.ContainsKey((int)StatsCategory.PrivUserEvents) ?
                          _thresholds[(int)StatsCategory.PrivUserEvents] : null;
                SetThreshold(_txtPrivUserWarning, _txtPrivUserError, _cbPrivUserPeriod, _checkPrivUser, current);

                current = _thresholds.ContainsKey((int)StatsCategory.Alerts) ?
                          _thresholds[(int)StatsCategory.Alerts] : null;
                SetThreshold(_txtAlertsWarning, _txtAlertsError, _cbAlertsPeriod, _checkAlerts, current);

                int temmpp = (int)StatsCategory.FailedLogin;
                current = _thresholds.ContainsKey((int)StatsCategory.FailedLogin) ?
                          _thresholds[(int)StatsCategory.FailedLogin] : null;
                SetThreshold(_txtFailedLoginsWarning, _txtFailedLoginsError, _cbFailedLoginsPeriod, _checkFailedLogins, current);

                current = _thresholds.ContainsKey((int)StatsCategory.DDL) ? _thresholds[(int)StatsCategory.DDL]
                             : null;
                SetThreshold(_txtDDLWarning, _txtDDLError, _cbDDLPeriod, _checkDDL, current);

                current = _thresholds.ContainsKey((int)StatsCategory.Security) ?
                          _thresholds[(int)StatsCategory.Security] : null;
                SetThreshold(_txtSecurityWarning, _txtSecurityError, _cbSecurityPeriod, _checkSecurity, current);

                current = _thresholds.ContainsKey((int)StatsCategory.EventProcessed) ?
                          _thresholds[(int)StatsCategory.EventProcessed] : null;
                SetThreshold(_txtAllActivityWarning, _txtAllActivityError, _cbAllActivityPeriod, _checkAllActivity, current);
                //start sqlcm 5.6 -5363
                current = _thresholds.ContainsKey((int)StatsCategory.Logins) ?
                          _thresholds[(int)StatsCategory.Logins] : null;
                SetThreshold(_txtLoginsWarning, _txtLoginsError, _cbLoginsPeriod, _checkLogins, current);

                current = _thresholds.ContainsKey((int)StatsCategory.Logout) ?
                          _thresholds[(int)StatsCategory.Logout] : null;
                SetThreshold(_txtLogoutsWarning, _txtLogoutsError, _cbLogoutsPeriod, _checkLogouts, current);
                //end sqlcm 5.6-5363
            }
            catch (Exception e)
            {
                MessageBox.Show(this, String.Format("Unable to load server thresholds:  {0}", e.Message),
                                "Error loading server thresholds");
                ErrorLog.Instance.Write("Unable to load server thresholds", e);
            }
        }

        /// <summary>
        /// This function updates the UI with the supplied threshold.
        /// </summary>
        /// <param name="txtWarning"></param>
        /// <param name="txtError"></param>
        /// <param name="cbPeriod">Combo for period - index 0 is per hour, index 1 is per day</param>
        /// <param name="record"></param>
        private static void SetThreshold(TextBox txtWarning, TextBox txtError, ComboBox cbPeriod, CheckBox checkBoxEnabled, ReportCardRecord record)
        {
            if (record == null)
            {
                txtWarning.Text = "100";
                txtError.Text = "150";
                cbPeriod.SelectedIndex = 0;
                checkBoxEnabled.Checked = false;
            }
            else
            {
                txtWarning.Text = record.WarningThreshold.ToString();
                txtError.Text = record.CriticalThreshold.ToString();
                if (record.Period == 4)
                    cbPeriod.SelectedIndex = 0;
                else
                    cbPeriod.SelectedIndex = 1;
                checkBoxEnabled.Checked = record.Enabled;
            }
        }

        /// <summary>
        /// Given a category, extract the current settings from the UI and create
        /// a report card entry for them.
        /// </summary>
        /// <param name="category"></param>
        /// <returns>A valid report card entry for this server and category</returns>
        private ReportCardRecord GetThreshold(StatsCategory category)
        {
            ReportCardRecord retVal = new ReportCardRecord(oldServer.SrvId, category);
            TextBox warning, error;
            ComboBox combo;
            CheckBox enabled;

            switch (category)
            {
                case StatsCategory.Alerts:
                    combo = _cbAlertsPeriod;
                    warning = _txtAlertsWarning;
                    error = _txtAlertsError;
                    enabled = _checkAlerts;
                    break;
                case StatsCategory.PrivUserEvents:
                    combo = _cbPrivUserPeriod;
                    warning = _txtPrivUserWarning;
                    error = _txtPrivUserError;
                    enabled = _checkPrivUser;
                    break;
                case StatsCategory.FailedLogin:
                    combo = _cbFailedLoginsPeriod;
                    warning = _txtFailedLoginsWarning;
                    error = _txtFailedLoginsError;
                    enabled = _checkFailedLogins;
                    break;
                case StatsCategory.DDL:
                    combo = _cbDDLPeriod;
                    warning = _txtDDLWarning;
                    error = _txtDDLError;
                    enabled = _checkDDL;
                    break;
                case StatsCategory.Security:
                    combo = _cbSecurityPeriod;
                    warning = _txtSecurityWarning;
                    error = _txtSecurityError;
                    enabled = _checkSecurity;
                    break;
                case StatsCategory.EventProcessed:
                    combo = _cbAllActivityPeriod;
                    warning = _txtAllActivityWarning;
                    error = _txtAllActivityError;
                    enabled = _checkAllActivity;
                    break;
                //start sqlcm 5.6 -5363 start
                case StatsCategory.Logins:
                    combo = _cbLoginsPeriod;
                    warning = _txtLoginsWarning;
                    error = _txtLoginsError;
                    enabled = _checkLogins;
                    break;
                case StatsCategory.Logout:
                    combo = _cbLogoutsPeriod;
                    warning = _txtLogoutsWarning;
                    error = _txtLogoutsError;
                    enabled = _checkLogouts;
                    break;
                //end sqlcm 5.6 -5363 end
                default:
                    return null;
            }
            if (combo.SelectedIndex == 0)
                retVal.Period = 4;
            else
                retVal.Period = 96;
            // We assume text validation occurred in the ValidateProperties function
            retVal.WarningThreshold = UIUtils.TextToInt(warning.Text);
            retVal.CriticalThreshold = UIUtils.TextToInt(error.Text);
            // If any of the thresholds are less than 0, we set them to -1, our flag
            //  for "Not Set"
            if (retVal.WarningThreshold < 0)
                retVal.WarningThreshold = -1;
            if (retVal.CriticalThreshold < 0)
                retVal.CriticalThreshold = -1;
            retVal.Enabled = enabled.Checked;

            return retVal;
        }

        /// <summary>
        /// This function saves the thresholds to the database if they have changed.
        /// </summary>
        /// <returns></returns>
        private void SaveThresholds()
        {
            ReportCardRecord newValue, origValue;

            origValue = _thresholds.ContainsKey((int)StatsCategory.PrivUserEvents) ?
                        _thresholds[(int)StatsCategory.PrivUserEvents] : null;
            newValue = GetThreshold(StatsCategory.PrivUserEvents);
            SaveThreshold(origValue, newValue);

            origValue = _thresholds.ContainsKey((int)StatsCategory.Alerts) ?
                        _thresholds[(int)StatsCategory.Alerts] : null;
            newValue = GetThreshold(StatsCategory.Alerts);
            SaveThreshold(origValue, newValue);

            origValue = _thresholds.ContainsKey((int)StatsCategory.FailedLogin) ?
                        _thresholds[(int)StatsCategory.FailedLogin] : null;
            newValue = GetThreshold(StatsCategory.FailedLogin);
            SaveThreshold(origValue, newValue);

            origValue = _thresholds.ContainsKey((int)StatsCategory.DDL) ?
                        _thresholds[(int)StatsCategory.DDL] : null;
            newValue = GetThreshold(StatsCategory.DDL);
            SaveThreshold(origValue, newValue);

            origValue = _thresholds.ContainsKey((int)StatsCategory.Security) ?
                        _thresholds[(int)StatsCategory.Security] : null;
            newValue = GetThreshold(StatsCategory.Security);
            SaveThreshold(origValue, newValue);

            origValue = _thresholds.ContainsKey((int)StatsCategory.EventProcessed) ?
                        _thresholds[(int)StatsCategory.EventProcessed] : null;
            newValue = GetThreshold(StatsCategory.EventProcessed);
            SaveThreshold(origValue, newValue);

            //start sqlcm 5.6 -5363
            origValue = _thresholds.ContainsKey((int)StatsCategory.Logins) ?
                        _thresholds[(int)StatsCategory.Logins] : null;
            newValue = GetThreshold(StatsCategory.Logins);
            SaveThreshold(origValue, newValue);

            origValue = _thresholds.ContainsKey((int)StatsCategory.Logout) ?
                    _thresholds[(int)StatsCategory.Logout] : null;
            newValue = GetThreshold(StatsCategory.Logout);
            SaveThreshold(origValue, newValue);
            //end sqlcm 5.6 -5363
        }

        /// <summary>
        /// Given two thresholds, this function saves the new value if the old and new values
        /// are different
        /// </summary>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        private void SaveThreshold(ReportCardRecord oldValue, ReportCardRecord newValue)
        {
            if (oldValue == null && newValue.Enabled)
            {
                // We have a new threshold to write
                newValue.Write(Globals.Repository.Connection);
                ThresholdsDirty = true;
            }
            else if (oldValue != null && (oldValue.Enabled != newValue.Enabled ||
                                         oldValue.CriticalThreshold != newValue.CriticalThreshold ||
                                         oldValue.WarningThreshold != newValue.WarningThreshold ||
                                         oldValue.Period != newValue.Period))
            {
                // We need to update an existing record
                newValue.Update(Globals.Repository.Connection);
                ThresholdsDirty = true;
            }
        }

        private void CheckedChanged_ThresholdEnabled(object sender, EventArgs e)
        {
            bool enabled;
            TextBox t1, t2;
            ComboBox c1;

            if (sender is CheckBox)
            {
                enabled = ((CheckBox)sender).Checked;
            }
            else
                return;
            if (sender == _checkAlerts)
            {
                t1 = _txtAlertsError;
                t2 = _txtAlertsWarning;
                c1 = _cbAlertsPeriod;
            }
            else if (sender == _checkAllActivity)
            {
                t1 = _txtAllActivityError;
                t2 = _txtAllActivityWarning;
                c1 = _cbAllActivityPeriod;
            }
            else if (sender == _checkDDL)
            {
                t1 = _txtDDLError;
                t2 = _txtDDLWarning;
                c1 = _cbDDLPeriod;
            }
            else if (sender == _checkFailedLogins)
            {
                t1 = _txtFailedLoginsError;
                t2 = _txtFailedLoginsWarning;
                c1 = _cbFailedLoginsPeriod;
            }
            else if (sender == _checkPrivUser)
            {
                t1 = _txtPrivUserError;
                t2 = _txtPrivUserWarning;
                c1 = _cbPrivUserPeriod;
            }
            else if (sender == _checkSecurity)
            {
                t1 = _txtSecurityError;
                t2 = _txtSecurityWarning;
                c1 = _cbSecurityPeriod;
            }
            //start sqlcm 5.6 - 5363
            else if (sender == _checkLogins)
            {
                t1 = _txtLoginsError;
                t2 = _txtLoginsWarning;
                c1 = _cbLoginsPeriod;
            }
            else if (sender == _checkLogouts)
            {
                t1 = _txtLogoutsError;
                t2 = _txtLogoutsWarning;
                c1 = _cbLogoutsPeriod;
            }
            //end sqlcm 5.6 - 5363
            else
                return;

            t1.Enabled = enabled;
            t2.Enabled = enabled;
            c1.Enabled = enabled;
        }

        private void linkLblHelpBestPractices_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            HelpAlias.ShowHelp(this, HelpAlias.SSHELP_AuditingBestPractices);
        }

        private void chkUserCaptureDDL_CheckedChanged(object sender, EventArgs e)
        {
            if (rbAuditUserSelected.Checked && (chkAuditUserDDL.Checked || chkAuditUserSecurity.Checked) && CoreConstants.AllowCaptureSql)
                chkUserCaptureDDL.Enabled = Globals.isAdmin;

            else
                chkUserCaptureDDL.Enabled = false;
        }


        private void auditOption_CheckedChanged(object sender, EventArgs e)
        {
            if (!isLoaded || radioTrace.Checked || !((RadioButton)sender).Checked)
                return;
            Cursor = Cursors.WaitCursor;
            try
            {
                if (radioXEvents.Checked)
                {
                    AgentManager manager = GUIRemoteObjectsProvider.AgentManager();
                    if (!manager.IsLinqAssemblyLoaded())
                    {
                        Linq_ErrorBox.Show();
                        radioTrace.Checked = true;
                        return;
                    }

                    //SQLcm 5.6 (Aakash Prakash) - Fix for 4967
                    if (ServerRecord.GetCountDatabasesAuditingAllObjects(Globals.Repository.Connection, oldServer.SrvId) != DatabaseRecord.GetCountDatabasesWithEnabledAuditDML(Globals.Repository.Connection, oldServer.SrvId))
                    {
                        ErrorMessage.Show(UIConstants.Caption_AuditingViaExtendedEvents, UIConstants.Error_AuditingViaExtendedEvents);
                        radioTrace.Checked = true;
                        return;
                    }

                }
                try
                {
                    AgentCommand agentCmd = GUIRemoteObjectsProvider.AgentCommand(oldServer.InstanceServer, oldServer.AgentPort);
                    agentCmd.Ping();
                }
                catch
                {
                    MessageBox.Show(CoreConstants.Error_AuditLogsAgentNotReachable, "Error",
                                  MessageBoxButtons.OK, MessageBoxIcon.Error);
                    radioTrace.Checked = true;
                    return;
                }
            }
            catch (SocketException ex)
            {
                string errorMsg = CoreConstants.Error_AuditLogsGenericErrorMessage;
                if (ex.ErrorCode == 10061)
                    errorMsg = String.Format(CoreConstants.Exception_ServerNotAvailable,
                                         Globals.SQLcomplianceConfig.Server);

                MessageBox.Show(errorMsg,
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                        ex,
                                        true);
                radioTrace.Checked = true;
            }
            catch (Exception ex)
            {
                string errorMsg = CoreConstants.Error_AuditLogsGenericErrorMessage;
                MessageBox.Show(errorMsg,
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                        ex,
                                        true);
                radioTrace.Checked = true;
            }
            finally
            {
                Cursor = Cursors.Arrow;
            }
        }

        //v5.6 SQLCM-5373
        #region Trusted Users
        private void Click_btnAddTrustedUser(object sender, EventArgs e)
        {
            Form_PrivUser frm = new Form_PrivUser(oldServer.Instance, false);
            //frm.MainForm = this.mainForm;                                                      
            if (DialogResult.OK == frm.ShowDialog())
            {
                lstTrustedUsers.BeginUpdate();

                lstTrustedUsers.SelectedItems.Clear();
                foreach (ListViewItem itm in frm.listSelected.Items)
                {
                    bool found = false;
                    foreach (ListViewItem s in lstTrustedUsers.Items)
                    {
                        //SQLCM-5581, 5582
                        _removedTrustedusers.RemoveLogin(s.Text);
                        _removedTrustedusers.RemoveServerRole(s.Text);
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
                        lstTrustedUsers.Items.Add(newItem);
                    }
                }

                lstTrustedUsers.EndUpdate();

                if (lstTrustedUsers.Items.Count > 0)
                {
                    lstTrustedUsers.TopItem.Selected = true;
                    btnRemoveTrustedUser.Enabled = true;
                }
                else
                {
                    btnRemoveTrustedUser.Enabled = false;
                }
            }
        }

        private void Click_btnRemoveTrustedUser(object sender, EventArgs e)
        {
            //SQLCM-5581, 5582
            var selectionLogic = new DeselectionLogic("Trusted Users",
                "You are removing Trusted Users. The action should:",
                "Trusted Users",
                "Deselect Server level only",
                DeselectOptions.CurrentLevelOnly,
                "Deselect Server and All Databases",
                DeselectOptions.OtherLevels);

            var frmSelectionLogicDialog = new Form_SelectionLogicDialog(selectionLogic);
            var isDeselected = DialogResult.OK == frmSelectionLogicDialog.ShowDialog();
            //SQLCM-5623 v5.6
            if (!isDeselected)
            {
                return;
            }

            DeselectValues deselection = new DeselectValues();
            deselection.DeselectOption = frmSelectionLogicDialog.SelectedDeselectOptions;
            
            //End SQLCM-5581, 5582
            if (lstTrustedUsers.SelectedItems.Count == 0)
            {
                btnRemoveTrustedUser.Enabled = false;
                return;
            }

            lstTrustedUsers.BeginUpdate();

            int ndx = lstTrustedUsers.SelectedIndices[0];

            foreach (ListViewItem priv in lstTrustedUsers.SelectedItems)
            {
                priv.Remove();
                // update the Remove Trusted Users List for Other Levels only
                // Since Remove Trusted Uses List will be used to update the database level
                if (deselection.DeselectOption != DeselectOptions.OtherLevels)
                {
                    continue;
                }
                //SQLCM-5581, 5582
                if (priv.ImageIndex == (int)AppIcons.Img16.Role)
                {
                    _removedTrustedusers.AddServerRole(priv.Text, priv.Text, (int)priv.Tag);
                }
                else
                {
                    _removedTrustedusers.AddLogin(priv.Text, (byte[])priv.Tag);
                }
                //SQLCM-5581, 5582 - END
            }

            lstTrustedUsers.EndUpdate();

            // reset selected item
            if (lstTrustedUsers.Items.Count != 0)
            {
                lstTrustedUsers.Focus();
                if (ndx >= lstTrustedUsers.Items.Count)
                {
                    lstTrustedUsers.Items[lstTrustedUsers.Items.Count - 1].Selected = true;
                }
                else
                    lstTrustedUsers.Items[ndx].Selected = true;
            }
            else
            {
                btnRemoveTrustedUser.Enabled = false;
            }
        }

        private void SelectedIndexChanged_lstTrustedUsers(object sender, EventArgs e)
        {
            if (lstTrustedUsers.SelectedItems.Count == 0)
            {
                btnRemoveTrustedUser.Enabled = false;
            }
            else
            {
                btnRemoveTrustedUser.Enabled = Globals.isAdmin;
            }
        }

        private void LoadTrustedUsers()
        {
            if (!SupportsTrustedUsers())
            {
                lblTrustedUserStatus.Text = CoreConstants.Feature_TrustedUserNotAvailableAgent;
                pnlTrustedUsers.Visible = false;
                return;
            }
            lstTrustedUsers.BeginUpdate();

            UserList userList = new UserList(oldServer.AuditTrustedUsersList);

            // Add logins
            foreach (Login l in userList.Logins)
            {
                ListViewItem vi = lstTrustedUsers.Items.Add(l.Name);
                vi.Tag = l.Sid;
                vi.ImageIndex = (int)AppIcons.Img16.WindowsUser;
            }

            // Add server roles
            foreach (ServerRole r in userList.ServerRoles)
            {
                ListViewItem vi = lstTrustedUsers.Items.Add(r.FullName);
                vi.Tag = r.Id;
                vi.ImageIndex = (int)AppIcons.Img16.Role;
            }

            lstTrustedUsers.EndUpdate();

            if (lstTrustedUsers.Items.Count > 0)
            {
                lstTrustedUsers.TopItem.Selected = Globals.isAdmin;
                btnRemoveTrustedUser.Enabled = Globals.isAdmin;
            }
            else
            {
                btnRemoveTrustedUser.Enabled = false;
            }
        }

        private string GetTrustedUserProperty()
        {
            int count = 0;

            UserList ul = new UserList();

            foreach (ListViewItem vi in lstTrustedUsers.Items)
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
        private bool SupportsTrustedUsers()
        {
            if (oldServer == null ||
               String.IsNullOrEmpty(oldServer.AgentVersion) ||
               oldServer.AgentVersion.StartsWith("1") ||
               oldServer.AgentVersion.StartsWith("2"))
                return false;
            else
                return true;
        }


        private void LinkClicked_lnkTrustedUserHelp(object sender, LinkLabelLinkClickedEventArgs e)
        {
            HelpAlias.ShowHelp(this, HelpAlias.SSHELP_Form_DatabaseProperties_TrustedUsers);
        }

        #endregion

        /// <summary>
        /// Update Ui Controls based on the properties
        /// </summary>
        public void UpdateUiControls(bool checkedValue, DeselectValues deselectValue)
        {
            var deselectOption = deselectValue.DeselectOption;
            // perform action on deselect options and property
            switch (deselectValue.DeselectControl)
            {
                case DeselectControls.ServerLogins:
                    this.UpdateDependentCheckboxes(this.chkAuditUserLogins, checkedValue, deselectOption);
                    break;
                case DeselectControls.ServerLogouts:
                    this.UpdateDependentCheckboxes(this.chkAuditUserLogouts, checkedValue, deselectOption);
                    break;
                case DeselectControls.ServerFailedLogins:
                    this.UpdateDependentCheckboxes(this.chkAuditUserFailedLogins, checkedValue, deselectOption);
                    break;
                case DeselectControls.ServerSecurityChanges:
                    this.UpdateDependentCheckboxes(this.chkAuditUserSecurity, checkedValue, deselectOption);
                    break;
                case DeselectControls.ServerDatabaseDefinition:
                    this.UpdateDependentCheckboxes(this.chkAuditUserDDL, checkedValue, deselectOption);
                    break;
                case DeselectControls.ServerAdministrativeActivities:
                    this.UpdateDependentCheckboxes(this.chkAuditUserAdmin, checkedValue, deselectOption);
                    break;
                case DeselectControls.ServerUserDefined:
                    this.UpdateDependentCheckboxes(this.chkAuditUserUserDefined, checkedValue, deselectOption);
                    break;
                case DeselectControls.ServerFilterEvents:
                    this.UpdateDependentCheckboxes(this._cbUserFilterAccessCheck, checkedValue, deselectOption);
                    if (checkedValue)
                    {
                        if (_cbFilterAccessCheck.Enabled)
                        {
                            // SQLCM-5661: Update the radio buttons passed and failed if the checkbox is mark checked on cancel
                            this.rbAuditSuccessfulOnly.Enabled = this.rbAuditFailedOnly.Enabled = true;
                        }
                        // For Unchecked Values we don't have to set the radio buttons
                        UpdateDependentRadioButtons(this._rbUserAuditPassed, this.rbAuditSuccessfulOnly.Checked, deselectOption);
                        UpdateDependentRadioButtons(this._rbUserAuditFailed, this.rbAuditFailedOnly.Checked, deselectOption);
                    }
                    else if (this._cbUserFilterAccessCheck.Checked)
                    {
                        this.chkExcludes_Click(this._cbUserFilterAccessCheck, null);
                    }
                    break;
                case DeselectControls.ServerFilterEventsPassOnly:
                    if (rbAuditSuccessfulOnly.Enabled)
                    {
                        // For Unchecked Values we don't have to set the radio buttons
                        UpdateDependentRadioButtons(this._rbUserAuditPassed, this.rbAuditSuccessfulOnly.Checked, DeselectOptions.None);
                        UpdateDependentRadioButtons(this._rbUserAuditFailed, this.rbAuditFailedOnly.Checked, DeselectOptions.None);
                    }
                    break;
                case DeselectControls.ServerFilterEventsFailedOnly:
                    if (rbAuditSuccessfulOnly.Enabled)
                    {
                        // For Unchecked Values we don't have to set the radio buttons
                        UpdateDependentRadioButtons(this._rbUserAuditPassed, this.rbAuditSuccessfulOnly.Checked, DeselectOptions.None);
                        UpdateDependentRadioButtons(this._rbUserAuditFailed, this.rbAuditFailedOnly.Checked, DeselectOptions.None);
                    }
                    break;
                case DeselectControls.ServerUserFilterEvents:
                    // SQLCM-5661: Update the radio buttons passed and failed if the checkbox is mark checked on cancel
                    if (checkedValue && _cbUserFilterAccessCheck.Enabled)
                    {
                        this._rbUserAuditPassed.Enabled = this._rbUserAuditFailed.Enabled = true;
                    }
                    break;
                case DeselectControls.ServerUserLogins:
                    break;
            }
        }

        /// <summary>
        /// Update Dependent control based on deselect options and event handlers
        /// </summary>
        private void UpdateDependentRadioButtons(RadioButton dependentControl, bool checkedValue, DeselectOptions deselectOptions)
        {
            switch (deselectOptions)
            {
                case DeselectOptions.CurrentLevelOnly:
                    dependentControl.Enabled = false;
                    break;
                case DeselectOptions.None:
                case DeselectOptions.OtherLevels:
                    dependentControl.Enabled = false;
                    dependentControl.Checked = checkedValue;
                    break;
            }
        }

        /// <summary>
        /// Update Dependent control based on deselect options and event handlers
        /// </summary>
        private void UpdateDependentCheckboxes(CheckBox dependentControl, bool checkedValue, DeselectOptions deselectOptions, EventHandler checkedChangedHandler = null)
        {
            switch (deselectOptions)
            {
                case DeselectOptions.CurrentLevelOnly:
                    dependentControl.Enabled = !checkedValue;
                    break;
                case DeselectOptions.None:
                case DeselectOptions.OtherLevels:
                    dependentControl.Enabled = !checkedValue;
                    if (checkedChangedHandler != null)
                    {
                        dependentControl.CheckedChanged -= checkedChangedHandler;
                    }
                    dependentControl.Checked = checkedValue;
                    if (checkedChangedHandler != null)
                    {
                        dependentControl.CheckedChanged += checkedChangedHandler;
                    }
                    break;
            }
        }
    }
}