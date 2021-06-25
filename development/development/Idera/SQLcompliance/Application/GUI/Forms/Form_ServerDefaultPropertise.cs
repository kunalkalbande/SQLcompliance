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
    using System.Text;

    /// <summary>
    /// Summary description for Form_ServerProperties.
    /// </summary>
    /// <remarks>
    /// SQLCM-5375 - 6.1.4.1-Greying Logic and Deselection Implementation
    /// </remarks>
    public partial class Form_ServerDefaultPropertise : Form, IDeselectionClient
    {
        public enum Context
        {
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
     
        private Dictionary<int, ReportCardRecord> _thresholds;
        //SQLCM-5581, 5582
        private UserList _removedTrustedusers = new UserList();
        private UserList _removedPrivelegedUsers = new UserList();
        private UserList originalPrivilegedUserList;
        private UserList originalTrustedUserList;
        private bool _removeTrustedUsersAtServerLevelOnly = false;
        StringBuilder props = new StringBuilder();
        StringBuilder values = new StringBuilder();
        AccessCheckFilter AccessCheckValue;

        // SQLCM-5375 - 6.1.4.1-Greying Logic and Deselection Implementation
        private DeselectionManager deselectionManager;
        #endregion

        #region Constructor / Dispose

        public Form_ServerDefaultPropertise()
        {
            this.deselectionManager = new DeselectionManager(this);
            this.deselectionManager.InDefaultAuditSettingsPage = true;
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();
            this.Icon = Resources.SQLcompliance_product_ico;

            _thresholds = new Dictionary<int, ReportCardRecord>();
            lstTrustedUsers.SmallImageList = AppIcons.AppImageList16(); // v5.6 SQLCM-5373
            lstPrivilegedUsers.SmallImageList = AppIcons.AppImageList16();
            Shown += OnLoad;

            LoadPropertise();
            //start sqlcm 5.6 - 5683
            grpPrivilegedUserActivity.Enabled = true;
            //end sqlcm 5.6 - 5683

            //start sqlcm 5.6 - 5746
            txtBxPrivilegedUser.GotFocus += GotFocusPriv;
            txtBxPrivilegedUser.LostFocus += LostFocusPriv;
            txtBxTrustedUserName.GotFocus += GotFocusTrusted;
            txtBxTrustedUserName.LostFocus += LostFocusTrusted;
            //end sqlcm 5.6 - 5746
        }
        //start sqlcm 5.6 - 5746
        private void GotFocusPriv(object sender,EventArgs e)
        {
            AcceptButton = btnAddPriv;
        }
        private void LostFocusPriv(object sender, EventArgs e)
        {
            AcceptButton = btnSave;
        }
        private void GotFocusTrusted(object sender, EventArgs e)
        {
            AcceptButton = btnAddTrustedUser;
        }
        private void LostFocusTrusted(object sender, EventArgs e)
        {
            AcceptButton = btnSave;
        }
        //end sqlcm 5.6 - 5746
        private void LoadPropertise(bool loadIderaDefaultSettings=false)
        {
            chkAuditLogins.Checked = false;
            chkAuditLogouts.Checked = false;
            chkAuditFailedLogins.Checked = false;
            chkAuditDDL.Checked = false;
            chkAuditAdmin.Checked = false;
            chkAuditSecurity.Checked = false;
            chkAuditUserDefined.Checked = false;
            _cbFilterAccessCheck.Checked = false;

            try
            {
                string cmdstr = GetSelectSQL(loadIderaDefaultSettings);

                using (SqlCommand cmd = new SqlCommand(cmdstr, Globals.Repository.Connection))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (loadIderaDefaultSettings == false)
                            {
                                chkAuditLogins.Checked = SQLHelpers.GetBool(reader, "auditLogins");
                                chkAuditLogouts.Checked = SQLHelpers.GetBool(reader, "auditLogouts");
                                chkAuditFailedLogins.Checked = SQLHelpers.GetBool(reader, "auditFailedLogins");
                                chkAuditDDL.Checked = SQLHelpers.GetBool(reader, "auditDDL");
                                chkAuditSecurity.Checked = SQLHelpers.GetBool(reader, "auditSecurity");
                                chkAuditAdmin.Checked = SQLHelpers.GetBool(reader, "auditAdmin");
                                chkAuditUserDefined.Checked = SQLHelpers.GetBool(reader, "auditUDE");
                                radioTrace.Checked = SQLHelpers.GetBool(reader, "auditTrace");
                                radioXEvents.Checked = SQLHelpers.GetBool(reader, "auditCaptureSQLXE");
                                radioAuditLogs.Checked = SQLHelpers.GetBool(reader, "isAuditLogEnabled");

                                var auditFailures = (AccessCheckFilter)SQLHelpers.GetByteToInt(reader, "auditFailures");
                                //by default it should be success and 
                                switch (auditFailures)
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

                                LoadPrivilegedUsers(SQLHelpers.GetString(reader, "auditUsersList"));
                                rbAuditUserAll.Checked = SQLHelpers.GetBool(reader, "auditUserAll");
                                rbAuditUserSelected.Checked = !rbAuditUserAll.Checked;
                                chkAuditUserLogins.Checked = SQLHelpers.GetBool(reader, "auditUserLogins");
                                chkAuditUserLogouts.Checked = SQLHelpers.GetBool(reader, "auditUserLogouts");
                                chkAuditUserFailedLogins.Checked = SQLHelpers.GetBool(reader, "auditUserFailedLogins");
                                chkAuditUserDDL.Checked = SQLHelpers.GetBool(reader, "auditUserDDL");
                                chkAuditUserSecurity.Checked = SQLHelpers.GetBool(reader, "auditUserSecurity");
                                chkAuditUserAdmin.Checked = SQLHelpers.GetBool(reader, "auditUserAdmin");
                                chkAuditUserDML.Checked = SQLHelpers.GetBool(reader, "auditUserDML");
                                chkAuditUserSELECT.Checked = SQLHelpers.GetBool(reader, "auditUserSELECT");
                                chkAuditUserUserDefined.Checked = SQLHelpers.GetBool(reader, "auditUserUDE");

                                var auditUserFailures = (AccessCheckFilter)SQLHelpers.GetByteToInt(reader, "auditUserFailures");
                                switch (auditUserFailures)
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

                                chkUserCaptureSQL.Checked = SQLHelpers.GetBool(reader, "auditUserCaptureSQL");
                                chkUserCaptureTrans.Checked = SQLHelpers.GetBool(reader, "auditUserCaptureTrans");
                                chkUserCaptureDDL.Checked = SQLHelpers.GetBool(reader, "auditUserCaptureDDL");

                                //DML only property
                                if (rbAuditUserSelected.Checked && chkAuditUserDML.Checked)
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

                                var defaultAccess = SQLHelpers.GetByteToInt(reader, "defaultAccess");
                                if (defaultAccess == 2)
                                    radioGrantAll.Checked = true;
                                else if (defaultAccess == 1)
                                    radioGrantEventsOnly.Checked = true;
                                else
                                    radioDeny.Checked = true;

                                var maxSqlLength = SQLHelpers.GetInt32(reader, "maxSqlLength");
                                /// maxsql
                                if (maxSqlLength < 0)
                                {
                                    radioUnlimitedSQL.Checked = true;
                                    textLimitSQL.Text = "512";
                                    textReportLimitSQL.Text = "32763";
                                }
                                else if (maxSqlLength > 0 && maxSqlLength < 32763)
                                {
                                    radioLimitSQL.Checked = true;
                                    textLimitSQL.Text = maxSqlLength.ToString();
                                    textReportLimitSQL.Text = maxSqlLength.ToString();
                                }
                                else
                                {
                                    radioLimitSQL.Checked = true;
                                    textLimitSQL.Text = maxSqlLength.ToString();
                                    textReportLimitSQL.Text = "32763";
                                }

                                LoadTrustedUsers(SQLHelpers.GetString(reader, "auditTrustedUsersList"));
                            }
                            else
                            {
                                //start sqlcm 5.6 - 5745
                                if (chkAuditLogins.Enabled)
                                    chkAuditLogins.Checked = SQLHelpers.GetBool(reader, "auditLogins");
                                if (chkAuditLogouts.Enabled)
                                    chkAuditLogouts.Checked = SQLHelpers.GetBool(reader, "auditLogouts");
                                if (chkAuditFailedLogins.Enabled)
                                    chkAuditFailedLogins.Checked = SQLHelpers.GetBool(reader, "auditFailedLogins");
                                if (chkAuditDDL.Enabled)
                                    chkAuditDDL.Checked = SQLHelpers.GetBool(reader, "auditDDL");
                                if (chkAuditSecurity.Enabled)
                                    chkAuditSecurity.Checked = SQLHelpers.GetBool(reader, "auditSecurity");
                                if (chkAuditAdmin.Enabled)
                                    chkAuditAdmin.Checked = SQLHelpers.GetBool(reader, "auditAdmin");
                                if (chkAuditUserDefined.Enabled)
                                    chkAuditUserDefined.Checked = SQLHelpers.GetBool(reader, "auditUDE");
                                if (radioTrace.Enabled)
                                    radioTrace.Checked = SQLHelpers.GetBool(reader, "auditTrace");
                                if (radioXEvents.Enabled)
                                    radioXEvents.Checked = SQLHelpers.GetBool(reader, "auditCaptureSQLXE");
                                if (radioAuditLogs.Enabled)
                                    radioAuditLogs.Checked = SQLHelpers.GetBool(reader, "isAuditLogEnabled");

                                var auditFailures = (AccessCheckFilter)SQLHelpers.GetByteToInt(reader, "auditFailures");
                                //by default it should be success and 
                                if (_cbFilterAccessCheck.Enabled)
                                {
                                    switch (auditFailures)
                                    {
                                        case AccessCheckFilter.FailureOnly:
                                            _cbFilterAccessCheck.Checked = true;
                                            rbAuditFailedOnly.Checked = true;
                                            rbAuditFailedOnly.Enabled = true;
                                            rbAuditSuccessfulOnly.Enabled = true;

                                            _cbUserFilterAccessCheck.Enabled = false;
                                            _cbUserFilterAccessCheck.Checked = true;
                                            _rbUserAuditFailed.Enabled = false;
                                            _rbUserAuditPassed.Enabled = false;
                                            _rbUserAuditFailed.Checked = true;
                                            break;
                                        case AccessCheckFilter.SuccessOnly:
                                            _cbFilterAccessCheck.Checked = true;
                                            rbAuditSuccessfulOnly.Checked = true;
                                            rbAuditFailedOnly.Enabled = true;
                                            rbAuditSuccessfulOnly.Enabled = true;

                                            _cbUserFilterAccessCheck.Enabled = false;
                                            _cbUserFilterAccessCheck.Checked = true;
                                            _rbUserAuditFailed.Enabled = false;
                                            _rbUserAuditPassed.Enabled = false;
                                            _rbUserAuditPassed.Checked = true;

                                            break;
                                        case AccessCheckFilter.NoFilter:
                                            _cbFilterAccessCheck.Checked = false;
                                            rbAuditFailedOnly.Enabled = false;
                                            rbAuditSuccessfulOnly.Enabled = false;
                                            break;
                                    }
                                }
                                //rbAuditSuccessfulOnly.Checked   = ! server.AuditFailures;
                                //rbAuditFailedOnly.Checked              = server.AuditFailures;

                                LoadPrivilegedUsers(SQLHelpers.GetString(reader, "auditUsersList"));


                                rbAuditUserAll.Checked = SQLHelpers.GetBool(reader, "auditUserAll");

                                rbAuditUserSelected.Checked = !rbAuditUserAll.Checked;

                                if (!chkAuditLogins.Checked)
                                {
                                    chkAuditUserLogins.Checked = SQLHelpers.GetBool(reader, "auditUserLogins");
                                    chkAuditUserLogins.Enabled = true;
                                }
                                else
                                {
                                    chkAuditUserLogins.Checked = true;
                                    chkAuditUserLogins.Enabled = false;
                                }
                                if (!chkAuditLogouts.Checked)
                                {
                                    chkAuditUserLogouts.Checked = SQLHelpers.GetBool(reader, "auditUserLogouts");
                                    chkAuditUserLogouts.Enabled = true;
                                }
                                else
                                {
                                    chkAuditUserLogouts.Checked = true;
                                    chkAuditUserLogouts.Enabled = false;
                                }
                                if (!chkAuditFailedLogins.Checked)
                                {
                                    chkAuditUserFailedLogins.Checked = SQLHelpers.GetBool(reader, "auditUserFailedLogins");
                                    chkAuditUserFailedLogins.Enabled = true;
                                }
                                else
                                {
                                    chkAuditUserFailedLogins.Checked = true;
                                    chkAuditUserFailedLogins.Enabled = false;
                                }
                                if (!chkAuditDDL.Checked)
                                {
                                    chkAuditUserDDL.Checked = SQLHelpers.GetBool(reader, "auditUserDDL");
                                    chkAuditUserDDL.Enabled = true;
                                }
                                else
                                {
                                    chkAuditUserDDL.Checked = true;
                                    chkAuditUserDDL.Enabled = false;
                                }
                                if (!chkAuditSecurity.Checked)
                                {
                                    chkAuditUserSecurity.Checked = SQLHelpers.GetBool(reader, "auditUserSecurity");
                                    chkAuditUserSecurity.Enabled = true;
                                }
                                else
                                {
                                    chkAuditUserSecurity.Checked = true;
                                    chkAuditUserSecurity.Enabled = false;
                                }
                                if (!chkAuditAdmin.Checked)
                                {
                                    chkAuditUserAdmin.Checked = SQLHelpers.GetBool(reader, "auditUserAdmin");
                                    chkAuditUserAdmin.Enabled = true;
                                }
                                else
                                {
                                    chkAuditUserAdmin.Checked = true;
                                    chkAuditUserAdmin.Enabled = false;
                                }


                                chkAuditUserDML.Checked = SQLHelpers.GetBool(reader, "auditUserDML");


                                chkAuditUserSELECT.Checked = SQLHelpers.GetBool(reader, "auditUserSELECT");
                                if (!chkAuditUserDefined.Checked)
                                {
                                    chkAuditUserUserDefined.Checked = SQLHelpers.GetBool(reader, "auditUserUDE");
                                    chkAuditUserUserDefined.Enabled = true;
                                }
                                else
                                {
                                    chkAuditUserUserDefined.Checked = true;
                                    chkAuditUserUserDefined.Enabled = false;
                                }

                                var auditUserFailures = (AccessCheckFilter)SQLHelpers.GetByteToInt(reader, "auditUserFailures");
                                if (!_cbFilterAccessCheck.Checked)
                                {
                                    switch (auditUserFailures)
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
                                    _cbUserFilterAccessCheck.Enabled = true;
                                    _rbUserAuditFailed.Enabled = true;
                                    _rbUserAuditPassed.Enabled = true;
                                }
                               

                                //DML only property
                                if (rbAuditUserSelected.Checked && chkAuditUserDML.Checked)
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

                                if (chkUserCaptureSQL.Enabled)
                                    chkUserCaptureSQL.Checked = SQLHelpers.GetBool(reader, "auditUserCaptureSQL");
                                if (chkUserCaptureTrans.Enabled)
                                    chkUserCaptureTrans.Checked = SQLHelpers.GetBool(reader, "auditUserCaptureTrans");
                                if (chkUserCaptureDDL.Enabled)
                                    chkUserCaptureDDL.Checked = SQLHelpers.GetBool(reader, "auditUserCaptureDDL");

                                var defaultAccess = SQLHelpers.GetByteToInt(reader, "defaultAccess");
                                if (defaultAccess == 2)
                                    radioGrantAll.Checked = true;
                                else if (defaultAccess == 1)
                                    radioGrantEventsOnly.Checked = true;
                                else
                                    radioDeny.Checked = true;

                                var maxSqlLength = SQLHelpers.GetInt32(reader, "maxSqlLength");
                                /// maxsql
                                if (maxSqlLength < 0)
                                {
                                    radioUnlimitedSQL.Checked = true;
                                    textLimitSQL.Text = "512";
                                    textReportLimitSQL.Text = "32763";
                                }
                                else if (maxSqlLength > 0 && maxSqlLength < 32763)
                                {
                                    radioLimitSQL.Checked = true;
                                    textLimitSQL.Text = maxSqlLength.ToString();
                                    textReportLimitSQL.Text = maxSqlLength.ToString();
                                }
                                else
                                {
                                    radioLimitSQL.Checked = true;
                                    textLimitSQL.Text = maxSqlLength.ToString();
                                    textReportLimitSQL.Text = "32763";
                                }

                                LoadTrustedUsers(SQLHelpers.GetString(reader, "auditTrustedUsersList"));
                                //end sqlcm 5.6 - 5745
                            }
                        }
                    }
                }


                grpAuditUserActivity.Enabled = !rbAuditUserAll.Checked;


                // Report Card Thresholds
                LoadThresholds(loadIderaDefaultSettings);

                //------------------------------------------------------
                // Make controls read only unless user has admin access
                //------------------------------------------------------
                if (!Globals.isAdmin)
                {
                    // other tabs
                    for (int i = 1; i < tabProperties.TabPages.Count; i++)
                    {
                        foreach (Control ctrl in tabProperties.TabPages[i].Controls)
                        {
                            ctrl.Enabled = false;
                        }
                    }

                    // change buttons
                    btnSave.Visible = false;
                    btnCancel.Text = "Close";
                    btnCancel.Enabled = true;
                    this.AcceptButton = btnCancel;
                }
                // make sure we start on general tab
                tabProperties.SelectedTab = tabPageAuditSettings;
            }
            catch (Exception e)
            {
            }

        }
        private static string GetSelectSQL(bool loadIderaDefault)
      {
          if(loadIderaDefault)
              return string.Format("select * from {0}..{1}", CoreConstants.RepositoryDatabase, CoreConstants.IderaDefaultServerPropertise);
          return string.Format("select * from {0}..{1}", CoreConstants.RepositoryDatabase, CoreConstants.DefaultServerPropertise);
      }
        #endregion

        #region Private Methods

        //-------------------------------------------------------------
        // ValidateProperties
        //-------------------------------------------------------------
        private bool ValidateProperties()
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
            if (ValidateProperties())
            {
                SaveThresholds();
                try
                {
                    using (SqlCommand cmd = GetSqlUpdateCommand())
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("" + ex);
                }
            }
            return true;
        }

       
        #endregion

        //--------------------------------------------------------------------
        // CreateDefaultServerSettingsInsert
        //--------------------------------------------------------------------
        private SqlCommand GetSqlUpdateCommand()
        {
            var cmd = new SqlCommand();
            #region
            string query = string.Format("update {0}..{1}", CoreConstants.RepositoryDatabase, CoreConstants.DefaultServerPropertise);
            cmd.CommandText = query + " set auditLogins=@auditLogins,auditLogouts=@auditLogouts,auditFailedLogins=@auditFailedLogins," +
            "auditDDL=@auditDDL,auditSecurity= @auditSecurity, auditAdmin =@auditAdmin," +
         "auditTrace =@auditTrace,auditUDE = @auditUDE , auditFailures= @auditFailures , auditCaptureSQLXE = @auditCaptureSQLXE," +
         "isAuditLogEnabled = @isAuditLogEnabled, auditUsersList = @auditUsersList," +
         "auditUserAll=@auditUserAll , auditUserLogins=@auditUserLogins,auditUserLogouts=@auditUserLogouts,auditUserFailedLogins=@auditUserFailedLogins," +
         "auditUserDDL= @auditUserDDL , auditUserSecurity=@auditUserSecurity, auditUserAdmin=@auditUserAdmin,auditUserDML = @auditUserDML," +
         "auditUserSELECT = @auditUserSELECT,auditUserUDE = @auditUserUDE,auditUserFailures=@auditUserFailures,auditUserCaptureSQL=@auditUserCaptureSQL," +
         "auditUserCaptureTrans=@auditUserCaptureTrans , auditUserCaptureDDL =@auditUserCaptureDDL,defaultAccess=@defaultAccess, maxSqlLength=@maxSqlLength, auditTrustedUsersList=@auditTrustedUsersList";
            cmd.Connection = Globals.Repository.Connection;

            cmd.Parameters.AddWithValue("@auditLogins", chkAuditLogins.Checked);
            cmd.Parameters.AddWithValue("@auditLogouts", chkAuditLogouts.Checked);
            cmd.Parameters.AddWithValue("@auditFailedLogins", chkAuditFailedLogins.Checked);
            cmd.Parameters.AddWithValue("@auditDDL", chkAuditDDL.Checked);
            cmd.Parameters.AddWithValue("@auditSecurity", chkAuditSecurity.Checked);
            cmd.Parameters.AddWithValue("@auditAdmin", chkAuditAdmin.Checked);
            cmd.Parameters.AddWithValue("@auditUDE", chkAuditUserDefined.Checked);
            cmd.Parameters.AddWithValue("@auditTrace", radioTrace.Checked);
            cmd.Parameters.AddWithValue("@auditCaptureSQLXE", radioXEvents.Checked);
            cmd.Parameters.AddWithValue("@isAuditLogEnabled", radioAuditLogs.Checked);
            var auditFailures = 0;
            if (_cbFilterAccessCheck.Checked)
            {
                if (rbAuditFailedOnly.Checked)
                    auditFailures = (int)AccessCheckFilter.FailureOnly;
                else
                    auditFailures = (int)AccessCheckFilter.SuccessOnly;
            }
            else
            {
                auditFailures = (int)AccessCheckFilter.NoFilter;
            }

            cmd.Parameters.AddWithValue("@auditFailures", auditFailures);
           
            cmd.Parameters.AddWithValue("@auditUsersList", GetPrivilegedUserProperty());
            cmd.Parameters.AddWithValue("@auditUserAll", rbAuditUserAll.Checked);
            cmd.Parameters.AddWithValue("@auditUserLogins", chkAuditUserLogins.Checked);
            cmd.Parameters.AddWithValue("@auditUserLogouts", chkAuditUserLogouts.Checked);
            cmd.Parameters.AddWithValue("@auditUserFailedLogins", chkAuditUserFailedLogins.Checked);
            cmd.Parameters.AddWithValue("@auditUserDDL", chkAuditUserDDL.Checked);
            cmd.Parameters.AddWithValue("@auditUserSecurity", chkAuditUserSecurity.Checked);
            cmd.Parameters.AddWithValue("@auditUserAdmin", chkAuditUserAdmin.Checked);
            cmd.Parameters.AddWithValue("@auditUserDML", chkAuditUserDML.Checked);
            cmd.Parameters.AddWithValue("@auditUserSELECT", chkAuditUserSELECT.Checked);
            cmd.Parameters.AddWithValue("@auditUserUDE", chkAuditUserUserDefined.Checked);
            var auditUserFailures = 0;
            if (_cbUserFilterAccessCheck.Checked)
            {
                if (_rbUserAuditFailed.Checked)
                    auditUserFailures = (int)AccessCheckFilter.FailureOnly;
                else
                    auditUserFailures = (int)AccessCheckFilter.SuccessOnly;
            }
            else
            {
                auditUserFailures = (int)AccessCheckFilter.NoFilter;
            }

            cmd.Parameters.AddWithValue("@auditUserFailures", auditUserFailures);
            cmd.Parameters.AddWithValue("@auditUserCaptureSQL", chkUserCaptureSQL.Enabled && chkUserCaptureSQL.Checked);
            cmd.Parameters.AddWithValue("@auditUserCaptureTrans", chkUserCaptureTrans.Enabled && chkUserCaptureTrans.Checked);
            cmd.Parameters.AddWithValue("@auditUserCaptureDDL", chkUserCaptureDDL.Enabled && chkUserCaptureDDL.Checked);

            var defaultAccess = 0; //if radioDeny.Checked = true then 0;
            if (radioGrantAll.Checked)
                defaultAccess = 2;
            else if (radioGrantEventsOnly.Checked)
                defaultAccess = 1;
                
            cmd.Parameters.AddWithValue("@defaultAccess", defaultAccess);
            var maxSqlLength = 0;
            if (radioLimitSQL.Checked)
            {
                maxSqlLength = UIUtils.TextToInt(textLimitSQL.Text);
            }
            else
            {
                maxSqlLength = -1;
            }
            cmd.Parameters.AddWithValue("@maxSqlLength", maxSqlLength);
            cmd.Parameters.AddWithValue("@auditTrustedUsersList", GetTrustedUserProperty());
            #endregion
            return cmd;
        }

        #region OK / Cancel / Apply

        //-------------------------------------------------------------
        // btnOK_Click
        //-------------------------------------------------------------
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (IsDialogRequired(CoreConstants.ConfirmServerDefaultAuditSettings))
            {
                Form_ConfirmDefaultAuditSettings frm2 = new Form_ConfirmDefaultAuditSettings();
                frm2.StartPosition = FormStartPosition.CenterParent;
                frm2.ShowDialog();
            }
           
            SaveServerRecord();
            SaveDefaultDatabaseRecord();//ravi
            this.Close();
        }
        private void LoadValuesDefaultDatabaseSettings(DatabaseRecord record)
        {
            string query = String.Format("select * from {0}", CoreConstants.RepositoryDefaultDatabaseSettings);
            try
            {
                using (SqlCommand cmd = new SqlCommand(query, Globals.Repository.Connection))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        reader.Read();
                        record.AuditDDL = (Boolean)reader[reader.GetOrdinal("auditDDL")];
                        record.AuditSecurity = (Boolean)reader[reader.GetOrdinal("auditSecurity")];
                        record.AuditSELECT = (Boolean)reader[reader.GetOrdinal("auditSELECT")];
                        record.AuditDML = (Boolean)reader[reader.GetOrdinal("auditDML")];
                        record.AuditAdmin = (Boolean)reader[reader.GetOrdinal("auditAdmin")];
                        record.AuditAccessCheck = (AccessCheckFilter)reader.GetByte(reader.GetOrdinal("auditFailures"));
                        record.AuditCaptureDDL = (Boolean)reader[reader.GetOrdinal("auditCaptureDDL")];
                        record.AuditCaptureSQL = (Boolean)reader[reader.GetOrdinal("auditCaptureSQL")];
                        record.AuditCaptureTrans = (Boolean)reader[reader.GetOrdinal("auditCaptureTrans")];
                        record.AuditUsersList = SQLHelpers.GetString(reader, "auditUsersList");
                        record.AuditPrivUsersList = SQLHelpers.GetString(reader, "auditPrivUsersList");
                        record.AuditUserAll = (Boolean)reader[reader.GetOrdinal("auditUserAll")];
                        record.AuditUserLogins = (Boolean)reader[reader.GetOrdinal("auditUserLogins")];
                        record.AuditUserLogouts = (Boolean)reader[reader.GetOrdinal("auditUserLogouts")];
                        record.AuditUserFailedLogins = (Boolean)reader[reader.GetOrdinal("auditUserFailedLogins")];
                        record.AuditUserAdmin = (Boolean)reader[reader.GetOrdinal("auditUserAdmin")];
                        record.AuditUserSELECT = (Boolean)reader[reader.GetOrdinal("auditUserSELECT")];
                        record.AuditUserDDL = (Boolean)reader[reader.GetOrdinal("auditUserDDl")];
                        record.AuditUserDML = (Boolean)reader[reader.GetOrdinal("auditUserDML")];
                        record.AuditUserUDE = (Boolean)reader[reader.GetOrdinal("auditUserUDE")];
                        record.AuditUserAccessCheck = (AccessCheckFilter)reader.GetByte(reader.GetOrdinal("auditUserFailures"));
                        record.AuditUserCaptureDDL = (Boolean)reader[reader.GetOrdinal("auditUserCaptureDDL")];
                        record.AuditUserCaptureSQL = (Boolean)reader[reader.GetOrdinal("auditUserCaptureSQL")];
                        record.AuditUserCaptureTrans = (Boolean)reader[reader.GetOrdinal("auditUserCaptureTrans")];
                    }
                }
            }
            catch(Exception e)
            {
                MessageBox.Show(e + "");
            }
        }
        private void SaveDefaultDatabaseRecord()
        {
            DatabaseRecord record = new DatabaseRecord();
            LoadValuesDefaultDatabaseSettings(record);
            UpdateDatabaseLevelTrustedUsers(record);
            try
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    string query = string.Format("update {0}..{1}", CoreConstants.RepositoryDatabase, CoreConstants.RepositoryDefaultDatabaseSettings);
                    cmd.CommandText = query + " set auditDDL=@auditDDL,auditSecurity= @auditSecurity, auditAdmin =@auditAdmin," +
                 "auditDML =@auditDML,auditSELECT = @auditSelect , auditFailures= @auditFailure , auditCaptureSQL = @auditCaptureSql," +
                 "auditCaptureTrans = @auditCaptureTrans,auditCaptureDDL =@auditCaptureDDL , auditUsersList = @auditUserlist,auditPrivUsersList=@auditPrivList," +
                 "auditUserAll=@userAll , auditUserLogins=@userlogin,auditUserLogouts=@userlogout,auditUserFailedLogins=@userfailedlog," +
                 "auditUserDDL= @userDDL , auditUserSecurity=@userSecurity, auditUserAdmin=@userAdmin,auditUserDML = @userDML," +
                 "auditUserSELECT = @userSelect,auditUserUDE = @userUDE,auditUserFailures=@userFailure,auditUserCaptureSQL=@usercapturesql," +
                 "auditUserCaptureTrans=@userCaptureTrans , auditUserCaptureDDL =@userCaptureDDL";
                    cmd.Connection = Globals.Repository.Connection;

                    cmd.Parameters.AddWithValue("@auditDDL", record.AuditDDL);
                    cmd.Parameters.AddWithValue("@auditSecurity", record.AuditSecurity);
                    cmd.Parameters.AddWithValue("@auditAdmin", record.AuditAdmin);
                    cmd.Parameters.AddWithValue("@auditDML", record.AuditDML);
                    cmd.Parameters.AddWithValue("@auditSelect", record.AuditSELECT);
                    cmd.Parameters.AddWithValue("@auditFailure", (int)record.AuditAccessCheck);
                    cmd.Parameters.AddWithValue("@auditCaptureSql", record.AuditCaptureDDL);
                    cmd.Parameters.AddWithValue("@auditCaptureTrans", record.AuditCaptureTrans);
                    cmd.Parameters.AddWithValue("@auditCaptureDDL",record.AuditCaptureDDL);
                    cmd.Parameters.AddWithValue("@auditUserlist", record.AuditUsersList);
                    cmd.Parameters.AddWithValue("@auditPrivList", record.AuditPrivUsersList);
                    cmd.Parameters.AddWithValue("@userAll", record.AuditUserAll);
                    cmd.Parameters.AddWithValue("@userlogin", record.AuditUserLogins);
                    cmd.Parameters.AddWithValue("@userlogout", record.AuditUserLogouts);
                    cmd.Parameters.AddWithValue("@userfailedlog", record.AuditUserFailedLogins);
                    cmd.Parameters.AddWithValue("@userDDL", record.AuditUserDDL);
                    cmd.Parameters.AddWithValue("@userSecurity", record.AuditUserSecurity);
                    cmd.Parameters.AddWithValue("@userAdmin", record.AuditUserAdmin);
                    cmd.Parameters.AddWithValue("@userDML", record.AuditUserDML);
                    cmd.Parameters.AddWithValue("@userSelect", record.AuditUserSELECT);
                    cmd.Parameters.AddWithValue("@userUDE", record.AuditUserUDE);
                    cmd.Parameters.AddWithValue("@userFailure", (int)record.AuditUserAccessCheck);
                    cmd.Parameters.AddWithValue("@usercapturesql", record.AuditUserCaptureSQL);
                    cmd.Parameters.AddWithValue("@userCaptureTrans", record.AuditUserCaptureTrans);
                    cmd.Parameters.AddWithValue("@userCaptureDDL", record.AuditUserCaptureDDL);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception: " + ex);
            }
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

                //txtAuditSettingsStatus.Text = UIConstants.Status_Requested;
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
            //if (rbAuditUserSelected.Checked && chkAuditUserDML.Checked && ServerRecord.CompareVersions(oldServer.AgentVersion, "3.5") >= 0)
            if (rbAuditUserSelected.Checked && chkAuditUserDML.Checked)
            
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
           LoadPrivilegedUsers(string auditUsersList)
        {
            RemoveAllItems(lstPrivilegedUsers);
            lstPrivilegedUsers.BeginUpdate();
            UserList userList = new UserList(auditUsersList);

            // Add logins
            foreach (Login l in userList.Logins)
            {
                ListViewItem vi = new ListViewItem(l.Name);
                vi.Tag = l.Sid;
                vi.ImageIndex = (int)AppIcons.Img16.WindowsUser;
                lstPrivilegedUsers.Items.Add(vi);
            }

            // Add server roles
            foreach (ServerRole r in userList.ServerRoles)
            {
                ListViewItem vi = new ListViewItem(r.FullName);
                vi.Tag = r.Id;
                vi.ImageIndex = (int)AppIcons.Img16.Role;
                lstPrivilegedUsers.Items.Add(vi);
            }
            originalPrivilegedUserList = userList;
            lstPrivilegedUsers.EndUpdate();
            lstPrivilegedUsers.Refresh();

            if (lstPrivilegedUsers.Items.Count > 0)
            {
                //start sqlcm 5.6 - 5683
                //grpPrivilegedUserActivity.Enabled = Globals.isAdmin;
                //end sqlcm 5.6 - 5683
                lstPrivilegedUsers.TopItem.Selected = Globals.isAdmin;
                btnRemovePriv.Enabled = Globals.isAdmin;
            }
            else
            {
                //start sqlcm 5.6 - 5683
                //grpPrivilegedUserActivity.Enabled = false;
                //end sqlcm 5.6 - 5683
                btnRemovePriv.Enabled = false;
            }
        }

        private byte[] ConvertIntToByteArray(int value)
        {
            byte[] intBytes = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(intBytes);
            return intBytes;
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
                    ul.AddServerRole(vi.Text, vi.Text, count);
                }
                else
                {
                    ul.AddLogin(vi.Text, ConvertIntToByteArray(count));
                }
            }

            return (count == 0) ? "" : ul.ToString();
        }

        private void btnAddPriv_Click(object sender, EventArgs e)
        {
            if (!txtBxPrivilegedUser.Text.Equals(""))
            {
                if (!CheckDuplicatePrivilegedUser(txtBxPrivilegedUser.Text)) //if login/role not exist then add
                {
                    //start sqlcm -5719
                    lstPrivilegedUsers.BeginUpdate();
                    lstPrivilegedUsers.SelectedItems.Clear();
                    //end sqlcm - 5719
                    ListViewItem item = new ListViewItem(txtBxPrivilegedUser.Text);
                    item.Name = txtBxPrivilegedUser.Text;
                    if (cmbBxPrivilegedUser.SelectedIndex == 1)
                        item.ImageIndex = (int)AppIcons.Img16.WindowsUser;
                    else
                        item.ImageIndex = (int)AppIcons.Img16.Role;

                    lstPrivilegedUsers.Items.Add(item);
                    txtBxPrivilegedUser.Text = string.Empty;

                    //start sqlcm 5.6 - 5683
                    //grpPrivilegedUserActivity.Enabled = true;
                    //end sqlcm 5.6 - 5683
                    lstPrivilegedUsers.TopItem.Selected = true;
                    btnRemovePriv.Enabled = true;

                    //start sqlcm 5.6 - 5719
                    lstPrivilegedUsers.EndUpdate();
                    //end sqlcm 5.6 - 5719
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

            //start sqlcm 5.6 - 5683
            // grpPrivilegedUserActivity.Enabled = (lstPrivilegedUsers.Items.Count != 0);
            //end sqlcm 5.6 - 5683

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
                //start sqlcm 5.6 - 5683
                // grpPrivilegedUserActivity.Enabled = false;
                //end sqlcm 5.6 - 5683
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

        private void Form_ServerDefaultProperties_Load(object sender, EventArgs e)
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

        public void OnLoad(object sender, EventArgs e)
        {
            if (IsDialogRequired(CoreConstants.AlertServerDefaultAuditSettings))
            {
                Form_AlertingDefaultAuditSettings frm = new Form_AlertingDefaultAuditSettings();
                frm.StartPosition = FormStartPosition.CenterParent;
                frm.ShowDialog();
            }
        }

        private bool IsDialogRequired(string flagName)
        {
            try
            {
                string sql = String.Format("SELECT isSet FROM {0}..{1} WHERE flagName = '"+flagName+"'",
                   CoreConstants.RepositoryDatabase,
                   CoreConstants.RepositoryDefaultAuditSettingDialogFlags);
                using (SqlCommand cmd = new SqlCommand(sql, Globals.Repository.Connection))
                {
                    bool count = (bool)cmd.ExecuteScalar();
                    return count;
                }
               
            }
            catch (Exception ex)
            {
                //log ex
                return false;
            }
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
        private void LoadThresholds(bool loadIderaDefaultSettings)
        {
            try
            {
                List<ReportCardRecord> records = ReportCardRecord.GetDefaultServerReportCardEntries(Globals.Repository.Connection,loadIderaDefaultSettings);
                ReportCardRecord current;
                _thresholds.Clear();
                foreach (ReportCardRecord record in records)
                    _thresholds.Add(record.StatisticId, record);

                current = _thresholds.ContainsKey((int)StatsCategory.PrivUserEvents) ?
                          _thresholds[(int)StatsCategory.PrivUserEvents] : null;
                SetThreshold(_txtPrivUserWarning, _txtPrivUserError, _cbPrivUserPeriod, _checkPrivUser, current);

                current = _thresholds.ContainsKey((int)StatsCategory.Alerts) ?
                          _thresholds[(int)StatsCategory.Alerts] : null;
                SetThreshold(_txtAlertsWarning, _txtAlertsError, _cbAlertsPeriod, _checkAlerts, current);

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
            ReportCardRecord retVal = new ReportCardRecord(category);
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
            if (oldValue == null)
            {
                // We have a new threshold to write
                newValue.WriteDefault(Globals.Repository.Connection);
                ThresholdsDirty = true;
            }
            else 
            {
                // We need to update an existing record
                newValue.UpdateDefault(Globals.Repository.Connection);
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
          /*  Cursor = Cursors.WaitCursor;
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
            }*/
        }

        //v5.6 SQLCM-5373
        #region Trusted Users
        private void Click_btnAddTrustedUser(object sender, EventArgs e)
        {
            if (!txtBxTrustedUserName.Text.Equals(""))
            {
                if (!CheckDuplicateTrustedUser(txtBxTrustedUserName.Text)) //if log/role not exists then add
                {
                    //start sqlcm 5.6 - 5719
                    lstTrustedUsers.BeginUpdate();
                    lstTrustedUsers.SelectedItems.Clear();
                    //end sqlcm 5.6 - 5719
                    ListViewItem item = new ListViewItem(txtBxTrustedUserName.Text);
                    item.Name = txtBxTrustedUserName.Text;
                    if (comboBox1.SelectedIndex == 1)
                        item.ImageIndex = (int)AppIcons.Img16.WindowsUser;
                    else
                        item.ImageIndex = (int)AppIcons.Img16.Role;

                    lstTrustedUsers.Items.Add(item);
                    txtBxTrustedUserName.Text = string.Empty;

                    lstTrustedUsers.TopItem.Selected = true;
                    btnRemoveTrustedUser.Enabled = true;

                    //start sqlcm 5.6 - 5719
                    lstTrustedUsers.EndUpdate();
                    //end sqlcm 5.6 - 5719
                }
            }
        }

        private bool CheckDuplicateTrustedUser(string text)
        {
            if (!lstTrustedUsers.Items.ContainsKey(text)) //if login or role doesn't exist
                return false; //no need to check further
            else
            {
                if (comboBox1.SelectedIndex == 1) // SelectedIndex = 1  =>  it is Login
                {
                    foreach (ListViewItem item in lstTrustedUsers.Items)
                    {
                        if (item.ImageIndex == (int)AppIcons.Img16.WindowsUser)
                        {
                            if (item.Text.Equals(text))
                            {
                                return true;
                            }
                        }
                    }
                }
                else // SelectedIndex != 1 (actually it should be 0) => it is role
                {
                    foreach (ListViewItem item in lstTrustedUsers.Items)
                    {
                        if (item.ImageIndex == (int)AppIcons.Img16.Role)
                        {
                            if (item.Text.Equals(text))
                            {
                                return true;
                            }
                        }
                    }
                }
                return false;
            }


        }
        private bool CheckDuplicatePrivilegedUser(String text)
        {
            if (!lstPrivilegedUsers.Items.ContainsKey(text))//if login or role doesn't exist
                return false;//no need to check further
            else
            {
                if (cmbBxPrivilegedUser.SelectedIndex == 1) // SelectedIndex = 1  =>  it is Login
                {
                    foreach (ListViewItem item in lstPrivilegedUsers.Items)
                    {
                        if (item.ImageIndex == (int)AppIcons.Img16.WindowsUser)
                        {
                            if (item.Text.Equals(text))
                            {
                                return true;
                            }
                        }
                    }
                }
                else // SelectedIndex != 1 (actually it should be 0) => it is role
                {
                    foreach (ListViewItem item in lstPrivilegedUsers.Items)
                    {
                        if (item.ImageIndex == (int)AppIcons.Img16.Role)
                        {
                            if (item.Text.Equals(text))
                            {
                                return true;
                            }
                        }
                    }
                }
                return false;
            }


        }
        private void Click_btnRemoveTrustedUser(object sender, EventArgs e)
        {
            if (lstTrustedUsers.SelectedItems.Count == 0)
            {
                lstTrustedUsers.Enabled = false;
                return;
            }

            lstTrustedUsers.BeginUpdate();

            int ndx = lstTrustedUsers.SelectedIndices[0];

            foreach (ListViewItem priv in lstTrustedUsers.SelectedItems)
            {
                priv.Remove();
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

        private void RemoveAllItems(ListView lstView)
        {
            lstView.BeginUpdate();
            foreach (ListViewItem lst in lstView.Items)
            {
                lst.Remove();
            }
            lstView.EndUpdate();
        }
        private void LoadTrustedUsers(string auditTrustedUsersList)
        {
            //if (!SupportsTrustedUsers())
            //{
            //    lblTrustedUserStatus.Text = CoreConstants.Feature_TrustedUserNotAvailableAgent;
            //    pnlTrustedUsers.Visible = false;
            //    return;
            //}
            RemoveAllItems(lstTrustedUsers);
            lstTrustedUsers.BeginUpdate();

            UserList userList = new UserList(auditTrustedUsersList);
            
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
            originalTrustedUserList = userList;
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
            byte[] cnt = {1,1};

            UserList ul = new UserList();

            foreach (ListViewItem vi in lstTrustedUsers.Items)
            {
                count++;
                if (vi.ImageIndex == (int)AppIcons.Img16.Role)
                {
                    ul.AddServerRole(vi.Text, vi.Text, count);
                }
                else
                {
                    ul.AddLogin(vi.Text, ConvertIntToByteArray(count));
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

        private void btnRestoreToIderaDefaultSettings_Click(object sender, EventArgs e)
        {
            LoadPropertise(true);
        }

        private void UpdateDatabaseLevelTrustedUsers(DatabaseRecord newDb)
        {
            if (string.IsNullOrEmpty(newDb.AuditUsersList))
            {
                newDb.AuditUsersList = this.GetTrustedUserProperty();
            }
            else
            {
                UserList currentAddedTrustedUsers = new UserList();
                currentAddedTrustedUsers.LoadFromString(this.GetTrustedUserProperty());


                foreach (Login orglog in originalTrustedUserList.Logins)
                {
                    int get = 0;
                    foreach (Login log in currentAddedTrustedUsers.Logins)
                    {
                        if (orglog.Name.Equals(log.Name))
                        {
                            get = 1;
                            break;
                        }
                    }
                    if (get == 0)
                        _removedTrustedusers.AddLogin(orglog);
                }

                foreach (ServerRole orgrole in originalTrustedUserList.ServerRoles)
                {
                    int get = 0;
                    foreach (ServerRole role in currentAddedTrustedUsers.ServerRoles)
                    {
                        // SQLCM-5868: Roles added to default server settings gets added twice at database level
                        if (orgrole.CompareName(role))
                        {
                            get = 1;
                            break;
                        }
                    }
                    if (get == 0)
                        _removedTrustedusers.AddServerRole(orgrole);
                }

                UserList dbTrustedUsers = new UserList();
                dbTrustedUsers.LoadFromString(newDb.AuditUsersList);
                bool userFound = false;
                int dbUserLoginCount = dbTrustedUsers.Logins.Length;
                int dbUserServerRoleCount = dbTrustedUsers.ServerRoles.Length;
                if (currentAddedTrustedUsers.Logins.Length > 0) // if new Trusted users are added. 
                {
                    foreach (Login srvLogin in currentAddedTrustedUsers.Logins)
                    {
                        if (dbUserLoginCount > 0)
                        {
                            userFound = false;
                            foreach (Login dbLogin in dbTrustedUsers.Logins)
                            {
                                if (srvLogin.Name.Equals(dbLogin.Name))
                                {
                                    userFound = true;
                                    break;
                                }
                            }
                            if (!userFound) dbTrustedUsers.AddLogin(srvLogin);
                        }
                        else
                        {
                            dbTrustedUsers.AddLogin(srvLogin);
                        }
                    }
                }
                if (currentAddedTrustedUsers.ServerRoles.Length > 0)
                {
                    foreach (ServerRole srvSr in currentAddedTrustedUsers.ServerRoles)
                    {
                        if (dbUserServerRoleCount > 0)
                        {
                            userFound = false;
                            foreach (ServerRole dbSr in dbTrustedUsers.ServerRoles)
                            {
                                // SQLCM-5868: Roles added to default server settings gets added twice at database level
                                if (srvSr.CompareName(dbSr))
                                {
                                    userFound = true;
                                    break;
                                }
                            }
                            if (!userFound) dbTrustedUsers.AddServerRole(srvSr);
                        }
                        else
                        {
                            dbTrustedUsers.AddServerRole(srvSr);
                        }
                    }
                }
                if (this._removedTrustedusers.Logins.Length > 0 || this._removedTrustedusers.ServerRoles.Length > 0)
                {
                    foreach (Login l in this._removedTrustedusers.Logins)
                    {
                        dbTrustedUsers.RemoveLogin(l.Name);
                    }
                    foreach (ServerRole sr in this._removedTrustedusers.ServerRoles)
                    {
                        dbTrustedUsers.RemoveServerRole(sr.Name);
                    }
                }
                newDb.AuditUsersList = dbTrustedUsers.ToString();
            }
        }
    }
}