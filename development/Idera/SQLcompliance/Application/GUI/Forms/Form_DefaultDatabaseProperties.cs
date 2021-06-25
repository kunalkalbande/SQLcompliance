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

namespace Idera.SQLcompliance.Application.GUI.Forms
{
    /// <summary>
    /// Summary description for Form_DatabaseProperties.
    /// </summary>
    /// <remarks>
    /// SQLCM-5375 - 6.1.4.1-Greying Logic and Deselection Implementation
    /// </remarks>
    public partial class Form_DefaultDatabaseProperties : Form, IDeselectionClient
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

        //start sqlcm 5.6 - 5467
        bool serverAuditLogins;
        bool serverAuditLogouts;
        bool serverAuditFailedLogins;
        bool serverAuditDDL;
        bool serverAuditSecurity;
        bool serverAuditAdmin;
        bool serverAuditUDE;
        bool serverAuditTrace;
        bool serverAuditCaptureSQLXE;
        String serverTrustedUserList;
        int serverAuditAccessCheck;
        bool serverAuditUserLogins;
        bool serverAuditUserLogouts;
        bool serverAuditUserFailedLogins;
        bool serverAuditUserDDL;
        bool serverAuditUserSecurity;
        bool serverAuditUserAdmin;
        bool serverAuditUserDML;
        bool serverAuditUserSelect;
        bool serverAuditUserUDE;
        int serverAuditUserAccessCheck;
        bool serverAuditUserCaptureSQL;
        bool serverAuditUserCaptureTrans;
        bool serverAuditUserCaptureDDL;
        String serverPrivilegedUserList;
        //end sqlcm 5.6 - 5467
        public DatabaseRecord _db;
        private string privUserList;
        private string trustedUserList;
        private List<DatabaseRecord> _alwaysOnDBList = new List<DatabaseRecord>();

        // SQLCM-5375 - 6.1.4.1-Greying Logic and Deselection Implementation
        private DeselectionManager deselectionManager;


        private bool _isLoaded = false;


        private SQLDirect _sqlServer = null;




        #endregion

        private void chkAuditDML_CheckedChanged(object sender, EventArgs e)
        {


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

            }
            else
            {


                _chkCaptureSQL.Enabled = false;
            }

        }


        public List<DatabaseRecord> AlwaysOnDBList
        {
            get { return _alwaysOnDBList; }
            set
            {
                _alwaysOnDBList = value;
            }
        }


        #region Constructor / Dispose

        public Form_DefaultDatabaseProperties()
        {
            InitializeComponent();
            GetDefaultServerProperties();
            LoadPropertise();
            
            // Initialize Deselction Manager
            this.deselectionManager = new DeselectionManager(this);
            this.deselectionManager.InDefaultAuditSettingsPage = true;
            //
            // Required for Windows Form Designer support
            //
            

            _sqlServer = new SQLDirect();
            this.Icon = Resources.SQLcompliance_product_ico;
            _lstTrustedUsers.SmallImageList = AppIcons.AppImageList16();
            lstPrivilegedUsers.SmallImageList = AppIcons.AppImageList16();

            Shown += OnLoad;
            Load += FormDatabasePropertiesLoad;
            //start sqlcm 5.6 - 5683
            grpPrivilegedUserActivity.Enabled = true;
            //end sqlcm 5.6 - 5683  

            //start sqlcm 5.6 - 5746
            textBox2.GotFocus += GotFocusPriv;
            textBox2.LostFocus += LostFocusPriv;
            textBox1.GotFocus += GotFocusTrusted;
            textBox1.LostFocus += LostFocusTrusted;
            //end sqlcm 5.6 - 5746
        }
        //start sqlcm 5.6 - 5746
        private void GotFocusPriv(object sender, EventArgs e)
        {
            AcceptButton = btnAddPriv;
        }
        private void LostFocusPriv(object sender, EventArgs e)
        {
            AcceptButton = _btnOK;
        }
        private void GotFocusTrusted(object sender, EventArgs e)
        {
            AcceptButton = _btnAddUser;
        }
        private void LostFocusTrusted(object sender, EventArgs e)
        {
            AcceptButton = _btnOK;
        }
        //end sqlcm 56. - 5746
        private string GetSelectSqlStatement(bool loadIderaDefaultSettings)
        {
            if (loadIderaDefaultSettings)
                return String.Format("select * from {0}..{1}", CoreConstants.RepositoryDatabase, CoreConstants.RepositoryIderaDefaultDatabaseSettings); ;

            return String.Format("select * from {0}..{1}", CoreConstants.RepositoryDatabase, CoreConstants.RepositoryDefaultDatabaseSettings);
        }

        private void LoadPropertise(bool loadIderaDefaultSettings = false)
        {
            #region
            String query = GetSelectSqlStatement(loadIderaDefaultSettings);
            SqlCommand sqlcmd = null;
            SqlDataReader reader = null;

            try
            {
                sqlcmd = new SqlCommand(query, Globals.Repository.Connection);
                reader = sqlcmd.ExecuteReader();
                

                if (reader != null)
                {
                    reader.Read();

                    // Audit Settings
                    if (loadIderaDefaultSettings == false)
                    {
                        _chkAuditDDL.Checked = (Boolean)reader[reader.GetOrdinal("auditDDL")];
                        _chkAuditSecurity.Checked = (Boolean)reader[reader.GetOrdinal("auditSecurity")];
                        _chkAuditAdmin.Checked = (Boolean)reader[reader.GetOrdinal("auditAdmin")];
                        _chkAuditDML.Checked = (Boolean)reader[reader.GetOrdinal("auditDML")];
                        _chkAuditSELECT.Checked = (Boolean)reader[reader.GetOrdinal("auditSELECT")];
                        _chkCaptureSQL.Checked = (Boolean)reader[reader.GetOrdinal("auditCaptureSQL")];
                        _chkCaptureTrans.Checked = (Boolean)reader[reader.GetOrdinal("auditCaptureTrans")];
                        chkDBCaptureDDL.Checked = (Boolean)reader[reader.GetOrdinal("auditCaptureDDL")];

                        switch ((AccessCheckFilter)reader.GetByte(reader.GetOrdinal("auditFailures")))
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
                        _cbFilterOnAccess.Checked = (AccessCheckFilter)reader.GetByte(reader.GetOrdinal("auditFailures")) != AccessCheckFilter.NoFilter;


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

                        }
                        else
                        {

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



                        privUserList = SQLHelpers.GetString(reader, "auditPrivUsersList");

                        // Tab: Audited Users
                        try
                        {
                            LoadPrivilegedUsers();
                        }
                        catch (Exception e)
                        {
                            MessageBox.Show("Problem loading Privileged User auditing information: " + e, "Error");
                        }

                        rbAuditUserAll.Checked = (Boolean)reader[reader.GetOrdinal("auditUserAll")];
                        rbAuditUserSelected.Checked = !(Boolean)reader[reader.GetOrdinal("auditUserAll")];
                        chkAuditUserLogins.Checked = (Boolean)reader[reader.GetOrdinal("auditUserLogins")];

                        // SQLCM-5375 - Capture Logout Events at Server level
                        chkAuditUserLogouts.Checked = (Boolean)reader[reader.GetOrdinal("auditUserLogouts")];
                        chkAuditUserFailedLogins.Checked = (Boolean)reader[reader.GetOrdinal("auditUserFailedLogins")];
                        chkAuditUserDDL.Checked = (Boolean)reader[reader.GetOrdinal("auditUserDDL")];
                        chkAuditUserSecurity.Checked = (Boolean)reader[reader.GetOrdinal("auditUserSecurity")];
                        chkAuditUserAdmin.Checked = (Boolean)reader[reader.GetOrdinal("auditUserAdmin")];
                        chkAuditUserDML.Checked = (Boolean)reader[reader.GetOrdinal("auditUserDML")];
                        chkAuditUserSELECT.Checked = (Boolean)reader[reader.GetOrdinal("auditUserSELECT")];
                        chkAuditUserUserDefined.Checked = (Boolean)reader[reader.GetOrdinal("auditUserUDE")];
                        switch ((AccessCheckFilter)reader.GetByte(reader.GetOrdinal("auditUserFailures")))
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

                        chkUserCaptureSQL.Checked = (Boolean)reader[reader.GetOrdinal("auditUserCaptureSQL")];
                        chkUserCaptureTrans.Checked = (Boolean)reader[reader.GetOrdinal("auditUserCaptureTrans")];
                        chkUserCaptureDDL.Checked = (Boolean)reader[reader.GetOrdinal("auditUserCaptureDDL")];



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


                        trustedUserList = SQLHelpers.GetString(reader, "auditUsersList");
                        try
                        {
                            // Trusted Users
                            LoadTrustedUsers();
                        }

                        catch (Exception e)
                        {
                            MessageBox.Show("Problem loading trusted users:  " + e, "Error");
                        }

                    }
                    else
                    {
                        //start sqlcm 5.6 - 5745
                        if(_chkAuditDDL.Enabled)
                        _chkAuditDDL.Checked = (Boolean)reader[reader.GetOrdinal("auditDDL")];
                        if(_chkAuditSecurity.Enabled)
                        _chkAuditSecurity.Checked = (Boolean)reader[reader.GetOrdinal("auditSecurity")];
                        if(_chkAuditAdmin.Enabled)
                        _chkAuditAdmin.Checked = (Boolean)reader[reader.GetOrdinal("auditAdmin")];
                        if(_chkAuditDML.Enabled)
                        _chkAuditDML.Checked = (Boolean)reader[reader.GetOrdinal("auditDML")];
                        if(_chkAuditSELECT.Enabled)
                        _chkAuditSELECT.Checked = (Boolean)reader[reader.GetOrdinal("auditSELECT")];
                        if(_chkCaptureSQL.Enabled)
                        _chkCaptureSQL.Checked = (Boolean)reader[reader.GetOrdinal("auditCaptureSQL")];
                        if(_chkCaptureTrans.Enabled)
                        _chkCaptureTrans.Checked = (Boolean)reader[reader.GetOrdinal("auditCaptureTrans")];
                        if(chkDBCaptureDDL.Enabled)
                        chkDBCaptureDDL.Checked = (Boolean)reader[reader.GetOrdinal("auditCaptureDDL")];


                        if (!_chkAuditDDL.Checked && serverAuditUserDDL == false)
                            chkAuditUserDDL.Enabled = true;
                        else if(_chkAuditDDL.Checked)
                        {
                            chkAuditUserDDL.Enabled = false;
                            chkAuditUserDDL.Checked = true;
                        }
                        if (!_chkAuditSecurity.Checked && serverAuditUserSecurity == false)
                            chkAuditUserSecurity.Enabled = true;
                        else if(_chkAuditSecurity.Checked)
                        {
                            chkAuditUserSecurity.Enabled = false;
                            chkAuditUserSecurity.Checked = true;
                        }
                        if (!_chkAuditAdmin.Checked && serverAuditUserAdmin == false)
                            chkAuditUserAdmin.Enabled = true;
                        else if(_chkAuditAdmin.Checked)
                        {
                            chkAuditUserAdmin.Checked = true;
                            chkAuditUserAdmin.Enabled = false;
                        }
                        if (!_chkAuditDML.Checked && serverAuditUserDML == false)
                            chkAuditUserDML.Enabled = true;
                        else if(_chkAuditDML.Checked)
                        {
                            chkAuditUserDML.Enabled = false;
                            chkAuditUserDML.Checked = true;
                        }
                        if (!_chkAuditSELECT.Checked && serverAuditUserSelect == false)
                            chkAuditUserSELECT.Enabled = true;
                        else if(_chkAuditSELECT.Checked)
                        {
                            chkAuditUserSELECT.Enabled = false;
                            chkAuditUserSELECT.Checked = true;
                        }
                        if (!_chkCaptureSQL.Checked && serverAuditUserCaptureSQL == false)
                            chkUserCaptureSQL.Enabled = true;
                        else if(_chkCaptureSQL.Checked)
                        {
                            chkUserCaptureSQL.Enabled = false;
                            chkUserCaptureSQL.Checked = true;
                        }
                        if (!_chkCaptureTrans.Checked && serverAuditUserCaptureTrans == false)
                            chkUserCaptureTrans.Enabled = true;
                        else if(_chkCaptureTrans.Checked)
                        {
                            chkUserCaptureTrans.Enabled = false;
                            chkUserCaptureTrans.Checked = true;
                        }
                        if (!chkDBCaptureDDL.Checked && serverAuditUserCaptureDDL == false)
                            chkUserCaptureDDL.Enabled = true;
                        else if(chkDBCaptureDDL.Checked)
                        {
                            chkUserCaptureDDL.Enabled = false;
                            chkUserCaptureDDL.Checked = true;
                        }


                        if (_cbFilterOnAccess.Enabled)
                        {
                            switch ((AccessCheckFilter)reader.GetByte(reader.GetOrdinal("auditFailures")))
                            {
                                case AccessCheckFilter.SuccessOnly:
                                    _cbFilterOnAccess.Checked = true;
                                    _rbPassed.Checked = true;
                                    _rbPassed.Enabled = true;
                                    _rbFailed.Enabled = true;

                                    if (serverAuditUserAccessCheck == (int)AccessCheckFilter.NoFilter)
                                    {
                                        _cbUserFilterAccessCheck.Enabled = false;
                                        _cbUserFilterAccessCheck.Checked = true;
                                        _rbUserAuditFailed.Enabled = false;
                                        _rbUserAuditPassed.Enabled = false;
                                        _rbUserAuditPassed.Checked = true;
                                    }
                                    break;
                                case AccessCheckFilter.NoFilter:
                                    _cbFilterOnAccess.Checked = false;
                                    _rbPassed.Enabled = false;
                                    _rbFailed.Enabled = false;
                                    break;
                                case AccessCheckFilter.FailureOnly:
                                    _cbFilterOnAccess.Checked = true;
                                    _rbFailed.Checked = true;
                                    _rbPassed.Enabled = true;
                                    _rbFailed.Enabled = true;

                                    if (serverAuditUserAccessCheck == (int)AccessCheckFilter.NoFilter)
                                    {
                                        _cbUserFilterAccessCheck.Enabled = false;
                                        _cbUserFilterAccessCheck.Checked = true;
                                        _rbUserAuditFailed.Enabled = false;
                                        _rbUserAuditPassed.Enabled = false;
                                        _rbUserAuditFailed.Checked = true;
                                    }
                                    break;
                            }
                        }
                     //   _cbFilterOnAccess.Checked = (AccessCheckFilter)reader.GetByte(reader.GetOrdinal("auditFailures")) != AccessCheckFilter.NoFilter;


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

                        }
                        else
                        {

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



                        privUserList = SQLHelpers.GetString(reader, "auditPrivUsersList");

                        // Tab: Audited Users
                        try
                        {
                            LoadPrivilegedUsers();
                        }
                        catch (Exception e)
                        {
                            MessageBox.Show("Problem loading Privileged User auditing information: " + e, "Error");
                        }

                        rbAuditUserAll.Checked = (Boolean)reader[reader.GetOrdinal("auditUserAll")];
                        rbAuditUserSelected.Checked = !(Boolean)reader[reader.GetOrdinal("auditUserAll")];
                        if(chkAuditUserLogins.Enabled)
                        chkAuditUserLogins.Checked = (Boolean)reader[reader.GetOrdinal("auditUserLogins")];

                        // SQLCM-5375 - Capture Logout Events at Server level
                        if(chkAuditUserLogins.Checked && chkAuditUserLogouts.Enabled)
                        chkAuditUserLogouts.Checked = (Boolean)reader[reader.GetOrdinal("auditUserLogouts")];
                        if(chkAuditUserFailedLogins.Enabled)
                        chkAuditUserFailedLogins.Checked = (Boolean)reader[reader.GetOrdinal("auditUserFailedLogins")];
                        if (chkAuditUserDDL.Enabled)
                            chkAuditUserDDL.Checked = (Boolean)reader[reader.GetOrdinal("auditUserDDL")];
                        
                        if(chkAuditUserSecurity.Enabled)
                        chkAuditUserSecurity.Checked = (Boolean)reader[reader.GetOrdinal("auditUserSecurity")];

                        if(chkAuditUserAdmin.Enabled)
                        chkAuditUserAdmin.Checked = (Boolean)reader[reader.GetOrdinal("auditUserAdmin")];

                        if(chkAuditUserDML.Enabled)
                        chkAuditUserDML.Checked = (Boolean)reader[reader.GetOrdinal("auditUserDML")];
                     
                        if(chkAuditUserSELECT.Enabled)
                        chkAuditUserSELECT.Checked = (Boolean)reader[reader.GetOrdinal("auditUserSELECT")];
                        if(chkAuditUserUserDefined.Enabled)
                        chkAuditUserUserDefined.Checked = (Boolean)reader[reader.GetOrdinal("auditUserUDE")];

                        if (_cbUserFilterAccessCheck.Enabled)
                        {
                            switch ((AccessCheckFilter)reader.GetByte(reader.GetOrdinal("auditUserFailures")))
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
                        }
                      



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
                        if (chkUserCaptureSQL.Enabled)
                            chkUserCaptureSQL.Checked = (Boolean)reader[reader.GetOrdinal("auditUserCaptureSQL")];
                        if (chkUserCaptureTrans.Enabled)
                            chkUserCaptureTrans.Checked = (Boolean)reader[reader.GetOrdinal("auditUserCaptureTrans")];
                        if (chkUserCaptureDDL.Enabled)
                            chkUserCaptureDDL.Checked = (Boolean)reader[reader.GetOrdinal("auditUserCaptureDDL")];

                        trustedUserList = SQLHelpers.GetString(reader, "auditUsersList");
                        try
                        {
                            // Trusted Users
                            LoadTrustedUsers();
                        }

                        catch (Exception e)
                        {
                            MessageBox.Show("Problem loading trusted users:  " + e, "Error");
                        }
                        //end sqlcm 5.6 - 5745
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Exception: " + e);

            }
            finally
            {
                if (sqlcmd != null)
                    sqlcmd.Dispose();
                if (reader != null)
                    reader.Close();

            }

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



                // change buttons
                _btnOK.Visible = false;
                _btnCancel.Text = "Close";
                this.AcceptButton = _btnCancel;
            }

            // _tabControl.Controls.SetChildIndex(this._tabAuditSettings, 1); //not working
            _tabControl.SelectedTab = this._tabAuditSettings;
            #endregion
        }

        public void OnLoad(object sender, EventArgs e)
        {
            if (GetPopupCheckValue("ALERT_DATABASE_DEFAULT_AUDIT_SETTINGS"))
            {
                Form_AlertDefaultDatabaseSettings frm = new Form_AlertDefaultDatabaseSettings();
                frm.StartPosition = FormStartPosition.CenterParent;
                frm.ShowDialog();

            }
        }

        public bool GetPopupCheckValue(string value)
        {
            string query = String.Format("select isSet from {0}..{1} where flagName = '{2}'", CoreConstants.RepositoryDatabase, CoreConstants.RepositoryDefaultAuditSettingDialogFlags, value);
            try
            {
                using (SqlCommand cmd = new SqlCommand(query, Globals.Repository.Connection))
                {

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        reader.Read();
                        if (reader.GetBoolean(0))
                            return true;
                        else
                            return false;

                    }

                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e + "");
                return false;
            }

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
            int accessCheckValue=0, userAccessCheckValue=0;
            if (_cbFilterOnAccess.Checked)
            {
                if (_rbPassed.Checked)
                    accessCheckValue = (int)AccessCheckFilter.SuccessOnly;
                else if (_rbFailed.Checked)
                    accessCheckValue = (int)AccessCheckFilter.FailureOnly;
            }
            else
                accessCheckValue = (int)AccessCheckFilter.NoFilter;

            if (_cbUserFilterAccessCheck.Checked)
            {
                if (_rbUserAuditPassed.Checked)
                    userAccessCheckValue = (int)AccessCheckFilter.SuccessOnly;
                else if (_rbUserAuditFailed.Checked)
                    userAccessCheckValue = (int)AccessCheckFilter.FailureOnly;
            }
            else
                userAccessCheckValue = (int)AccessCheckFilter.NoFilter;




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

                    cmd.Parameters.AddWithValue("@auditDDL", _chkAuditDDL.Checked);
                    cmd.Parameters.AddWithValue("@auditSecurity", _chkAuditSecurity.Checked);
                    cmd.Parameters.AddWithValue("@auditAdmin", _chkAuditAdmin.Checked);
                    cmd.Parameters.AddWithValue("@auditDML", _chkAuditDML.Checked);
                    cmd.Parameters.AddWithValue("@auditSelect", _chkAuditSELECT.Checked);
                    cmd.Parameters.AddWithValue("@auditFailure", accessCheckValue);
                    cmd.Parameters.AddWithValue("@auditCaptureSql", _chkCaptureSQL.Checked);
                    cmd.Parameters.AddWithValue("@auditCaptureTrans", _chkCaptureTrans.Checked);
                    cmd.Parameters.AddWithValue("@auditCaptureDDL", chkDBCaptureDDL.Checked);
                    cmd.Parameters.AddWithValue("@auditUserlist", GetTrustedUserProperty());
                    cmd.Parameters.AddWithValue("@auditPrivList", GetPrivilegedUserProperty());
                    cmd.Parameters.AddWithValue("@userAll", rbAuditUserAll.Checked);
                    cmd.Parameters.AddWithValue("@userlogin", chkAuditUserLogins.Checked);
                    cmd.Parameters.AddWithValue("@userlogout", chkAuditUserLogouts.Checked);
                    cmd.Parameters.AddWithValue("@userfailedlog", chkAuditUserFailedLogins.Checked);
                    cmd.Parameters.AddWithValue("@userDDL", chkAuditUserDDL.Checked);
                    cmd.Parameters.AddWithValue("@userSecurity", chkAuditUserSecurity.Checked);
                    cmd.Parameters.AddWithValue("@userAdmin", chkAuditUserAdmin.Checked);
                    cmd.Parameters.AddWithValue("@userDML", chkAuditUserDML.Checked);
                    cmd.Parameters.AddWithValue("@userSelect", chkAuditUserSELECT.Checked);
                    cmd.Parameters.AddWithValue("@userUDE", chkAuditUserUserDefined.Checked);
                    cmd.Parameters.AddWithValue("@userFailure", userAccessCheckValue);
                    cmd.Parameters.AddWithValue("@usercapturesql", chkUserCaptureSQL.Checked);
                    cmd.Parameters.AddWithValue("@userCaptureTrans", chkUserCaptureTrans.Checked);
                    cmd.Parameters.AddWithValue("@userCaptureDDL", chkUserCaptureDDL.Checked);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception: " + ex);
            }
            if (GetPopupCheckValue("CONFIRM_DATABASE_DEFAULT_AUDIT_SETTINGS"))
            {
                Form_AlertSaveDefaultDatabaseSettings frm = new Form_AlertSaveDefaultDatabaseSettings();
                frm.StartPosition = FormStartPosition.CenterScreen;
                frm.ShowDialog();
            }
            this.Close();
        }

        #endregion



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






        #region Help
        //--------------------------------------------------------------------
        // Form_DatabaseProperties_HelpRequested - Show Context Sensitive Help
        //--------------------------------------------------------------------
        private void Form_DatabaseProperties_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            string helpTopic;


            if (_tabControl.SelectedTab == _tabAuditSettings)
                helpTopic = HelpAlias.SSHELP_Form_DatabaseProperties_Activities;
            else
                helpTopic = HelpAlias.SSHELP_Form_DatabaseProperties_TrustedUsers;



            HelpAlias.ShowHelp(this, helpTopic);
            hlpevent.Handled = true;
        }
        #endregion







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
        private bool CheckDuplicateTrustedUser(String text)
        {
            UserList serverTrustedUsers = new UserList(serverTrustedUserList);
            if (comboBox1.SelectedIndex == 1) // SelectedIndex = 1  =>  it is Login
            {
                foreach (ListViewItem item in _lstTrustedUsers.Items)
                {
                    if (item.ImageIndex == (int)AppIcons.Img16.WindowsUser)
                    {
                        if (item.Text.Equals(text))
                        {
                            return true;
                        }
                    }
                }

                foreach (var login in serverTrustedUsers.Logins)
                {
                    if (login.Name.Equals(text))
                    {
                        return true;
                    }
                }
            }
            else // SelectedIndex != 1 (actually it should be 0) => it is role
            {
                foreach (ListViewItem item in _lstTrustedUsers.Items)
                {
                    if (item.ImageIndex == (int)AppIcons.Img16.Role)
                    {
                        if (item.Text.Equals(text))
                        {
                            return true;
                        }
                    }
                }

                foreach (var role in serverTrustedUsers.ServerRoles)
                {
                    if (role.Name.Equals(text))
                    {
                        return true;
                    }
                }
            }
            return false;

        }
        private bool CheckDuplicatePrivilegedUser(String text)
        {
            UserList serverPrivilegedUsers = new UserList(serverPrivilegedUserList);

            if (comboBox2.SelectedIndex == 1) // SelectedIndex = 1  =>  it is Login
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

                foreach (var login in serverPrivilegedUsers.Logins)
                {
                    if (login.Name.Equals(text))
                    {
                        return true;
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

                foreach (var role in serverPrivilegedUsers.ServerRoles)
                {
                    if (role.Name.Equals(text))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        #region Trusted Users
        private void Click_btnAddUser(object sender, EventArgs e)
        {
            if (!textBox1.Text.Equals(""))
            {
                if (!CheckDuplicateTrustedUser(textBox1.Text))
                {//start sqlcm-5.6 - 5719
                    _lstTrustedUsers.BeginUpdate();
                    _lstTrustedUsers.SelectedItems.Clear();
                    //end sqlcm 5.6 - 5719
                    ListViewItem item = new ListViewItem(textBox1.Text);
                    item.Name = textBox1.Text;
                    if (comboBox1.SelectedIndex == 1)
                        item.ImageIndex = (int)AppIcons.Img16.WindowsUser;
                    else
                        item.ImageIndex = (int)AppIcons.Img16.Role;
                    _lstTrustedUsers.Items.Add(item);
                    textBox1.Text = string.Empty;

                    _lstTrustedUsers.TopItem.Selected = true;
                    _btnRemoveUser.Enabled = true;
                    //start sqlcm 5.6 - 5719
                    _lstTrustedUsers.EndUpdate();
                    //end sqlcm 5.6 - 5719

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
                UserList serverUserList = new UserList(serverTrustedUserList);
                var selectedItem = _lstTrustedUsers.SelectedItems[0];
                if (selectedItem.ImageIndex == 1) //imageindex = 1 => Login
                {
                    foreach (Login l in serverUserList.Logins)
                    {
                        if (selectedItem.Text.Equals(l.Name))
                        {
                            _btnRemoveUser.Enabled = false;
                            break;
                        }
                    }
                }
                else //imageindex != 1 => role
                {
                    foreach (ServerRole l in serverUserList.ServerRoles)
                    {
                        if (selectedItem.Text.Equals(l.Name))
                        {
                            _btnRemoveUser.Enabled = false;
                            break;
                        }
                    }
                }

                // Adding changes to include Trusted Users
                foreach (ListViewItem priv in _lstTrustedUsers.SelectedItems)
                {
                    if (priv.ForeColor == System.Drawing.Color.Gray)
                    {
                        priv.Selected = false;
                    }
                }
            }
        }

        private void RemoveAllItems(ListView lstView)
        {
            lstView.BeginUpdate();
            foreach (ListViewItem lst in lstView.Items)
            {
                if(lst.ForeColor!= System.Drawing.Color.Gray)
                lst.Remove();
            }
            lstView.EndUpdate();
        }

        private void LoadTrustedUsers()
        {
            int old_items = _lstTrustedUsers.Items.Count;
            RemoveAllItems(_lstTrustedUsers);
            UserList serverTrustedUsers = new UserList(serverTrustedUserList);
            _lstTrustedUsers.BeginUpdate();
           
            UserList userList = new UserList(trustedUserList);

            // Add logins
            foreach (Login l in userList.Logins)
            {
                ListViewItem vi = _lstTrustedUsers.Items.Add(l.Name);
                vi.Tag = l.Sid;
                vi.ImageIndex = (int)AppIcons.Img16.WindowsUser;
                if (serverTrustedUsers.Logins.Any(serverTrustedUser => serverTrustedUser.Name == l.Name))
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

            //start sqlcm 5.6 - 5745
            _btnRemoveUser.Enabled = false;
            for(int i=0;i<_lstTrustedUsers.Items.Count;i++)
            {
                if(_lstTrustedUsers.Items[i].ForeColor != System.Drawing.Color.Gray)
                {
                    _lstTrustedUsers.Items[i].Selected = true;
                    _btnRemoveUser.Enabled=true;
                    break;
                }
            }
         /*   if (_lstTrustedUsers.Items.Count > 0)
            {
                _lstTrustedUsers.TopItem.Selected = Globals.isAdmin;
                _btnRemoveUser.Enabled = Globals.isAdmin;
            }
            else
            {
                _btnRemoveUser.Enabled = false;
            }*/
            //end sqlcm 5.6 - 5745
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
                    ul.AddServerRole(vi.Text, vi.Text, 0);
                }
                else
                {
                    ul.AddLogin(vi.Text, new byte[] { 0 });
                }
            }

            return (count == 0) ? "" : ul.ToString();
        }

        private void LinkClicked_lnkTrustedUserHelp(object sender, LinkLabelLinkClickedEventArgs e)
        {
            HelpAlias.ShowHelp(this, HelpAlias.SSHELP_Form_DatabaseProperties_TrustedUsers);
        }

        #endregion



        private void linkLblHelpBestPractices_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            HelpAlias.ShowHelp(this, HelpAlias.SSHELP_AuditingBestPractices);
        }




        #region Privileged User Handling

        //---------------------------------------------------------------------------
        // LoadPrivilegedUsers - loads server roles and users
        //---------------------------------------------------------------------------
        private void
           LoadPrivilegedUsers()
        {
            RemoveAllItems(lstPrivilegedUsers);
            // SQLCM-5375 Greying logic for privileged users
            UserList serverPrivilegedUsers = new UserList(serverPrivilegedUserList);

            lstPrivilegedUsers.BeginUpdate();

            UserList userList = new UserList(privUserList);

            // Add logins
            foreach (Login l in userList.Logins)
            {
                ListViewItem vi = lstPrivilegedUsers.Items.Add(l.Name);
                vi.Tag = l.Sid;
                vi.ImageIndex = (int)AppIcons.Img16.WindowsUser;
                // SQLCM-5375 Greying logic for privileged users
                if (serverPrivilegedUsers.Logins.Any(serverLogin => serverLogin.Name == l.Name))
                {
                    vi.ForeColor = System.Drawing.Color.Gray;
                    vi.UseItemStyleForSubItems = true;
                }
            }

            // Add server roles
            foreach (ServerRole r in userList.ServerRoles)
            {
                ListViewItem vi = lstPrivilegedUsers.Items.Add(r.FullName);
                vi.Tag = r.Id;
                vi.ImageIndex = (int)AppIcons.Img16.Role;
                // SQLCM-5375 Greying logic for privileged users
                if (serverPrivilegedUsers.ServerRoles.Any(serverSr => serverSr.CompareName(r)))
                {
                    vi.ForeColor = System.Drawing.Color.Gray;
                    vi.UseItemStyleForSubItems = true;
                }
            }

            lstPrivilegedUsers.EndUpdate();

            //start sqlcm 5.6 - 5745
            btnRemovePriv.Enabled = false;
            for(int i=0;i<lstPrivilegedUsers.Items.Count;i++)
            {
                if(lstPrivilegedUsers.Items[i].ForeColor != System.Drawing.Color.Gray)
                {
                    lstPrivilegedUsers.Items[i].Selected = true;
                    btnRemovePriv.Enabled = true;
                    break;
                }
            }
            /*if (lstPrivilegedUsers.Items.Count > 0)
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
            }*/

            //end sqlcm 5.6 - 5745
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
                    ul.AddServerRole(vi.Text, vi.Text, 0);
                }
                else
                {
                    ul.AddLogin(vi.Text, new byte[] { 0 });
                }
            }

            return (count == 0) ? "" : ul.ToString();
        }

        private void btnAddPriv_Click(object sender, EventArgs e)
        {
            if (!textBox2.Text.Equals(""))
            {
                if (!CheckDuplicatePrivilegedUser(textBox2.Text)) //if login/role not exist then add
                {
                    //start sqlcm 5.6 - 5719
                    lstPrivilegedUsers.BeginUpdate();
                    lstPrivilegedUsers.SelectedItems.Clear();
                    //end sqlcm 5.6 - 5719
                    ListViewItem item = new ListViewItem(textBox2.Text);
                    item.Name = textBox2.Text;
                    if (comboBox2.SelectedIndex == 1)
                        item.ImageIndex = (int)AppIcons.Img16.WindowsUser;
                    else
                        item.ImageIndex = (int)AppIcons.Img16.Role;
                    lstPrivilegedUsers.Items.Add(item);
                    textBox2.Text = string.Empty;

                    //start sqlcm 5.6 - 5683
                   // grpPrivilegedUserActivity.Enabled = true;
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
            //grpPrivilegedUserActivity.Enabled = (lstPrivilegedUsers.Items.Count != 0);
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

                if (lstPrivilegedUsers.SelectedItems.Count != 0)
                    btnRemovePriv.Enabled = true;
            }
            else
            {
                //start sqlcm 5.6 - 5683
                //grpPrivilegedUserActivity.Enabled = false;
                //end sqlcm 5.6 - 5683
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
            if (rbAuditUserSelected.Checked && chkAuditUserDML.Checked)
                chkUserCaptureTrans.Enabled = Globals.isAdmin;  // Handle enabling only considering Server Level - User Privilege property
            else
                chkUserCaptureTrans.Enabled = false;

            //DML or SELECT property
            if (rbAuditUserSelected.Checked && (chkAuditUserDML.Checked || chkAuditUserSELECT.Checked) && CoreConstants.AllowCaptureSql)
                chkUserCaptureSQL.Enabled = Globals.isAdmin;  // Handle enabling only considering Server Level - User Privilege property
            else
                chkUserCaptureSQL.Enabled = false;
        }

        private void chkAuditUserDDL_CheckedChanged(object sender, EventArgs e)
        {
            if (rbAuditUserSelected.Checked && (chkAuditUserDDL.Checked || chkAuditUserSecurity.Checked) && CoreConstants.AllowCaptureSql)
                chkUserCaptureDDL.Enabled = Globals.isAdmin;  // Handle enabling only considering Server Level - User Privilege property
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

        private void GetDefaultServerProperties()
        {
            
            string query = String.Format("select * from {0}", CoreConstants.DefaultServerPropertise);
            try
            {
                using (SqlCommand cmd = new SqlCommand(query, Globals.Repository.Connection))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        reader.Read();
                        serverAuditLogins = (Boolean)reader[reader.GetOrdinal("auditLogins")];
                        serverAuditLogouts = (Boolean)reader[reader.GetOrdinal("auditLogouts")];
                        serverAuditFailedLogins = (Boolean)reader[reader.GetOrdinal("auditFailedLogins")];
                        serverAuditDDL = (Boolean)reader[reader.GetOrdinal("auditDDL")];
                        serverAuditSecurity = (Boolean)reader[reader.GetOrdinal("auditSecurity")];
                        serverAuditAdmin = (Boolean)reader[reader.GetOrdinal("auditAdmin")];
                        serverAuditUDE = (Boolean)reader[reader.GetOrdinal("auditUDE")];
                        serverAuditTrace = (Boolean)reader[reader.GetOrdinal("auditTrace")];
                        serverAuditCaptureSQLXE = (Boolean)reader[reader.GetOrdinal("auditCaptureSQLXE")];
                        serverTrustedUserList = SQLHelpers.GetString(reader, "auditTrustedUsersList");
                        serverAuditAccessCheck = (int)reader.GetByte(reader.GetOrdinal("auditFailures"));
                        serverAuditUserLogins = (Boolean)reader[reader.GetOrdinal("auditUserLogins")];
                        serverAuditUserLogouts = (Boolean)reader[reader.GetOrdinal("auditUserLogouts")];
                        serverAuditUserFailedLogins = (Boolean)reader[reader.GetOrdinal("auditUserFailedLogins")];
                        serverAuditUserDDL = (Boolean)reader[reader.GetOrdinal("auditUserDDL")];
                        serverAuditUserDML = (Boolean)reader[reader.GetOrdinal("auditUserDML")];
                        serverAuditUserSelect = (Boolean)reader[reader.GetOrdinal("auditUserSELECT")];
                        serverAuditUserUDE = (Boolean)reader[reader.GetOrdinal("auditUserUDE")];
                        serverAuditUserAccessCheck = (int)reader.GetByte(reader.GetOrdinal("auditUserFailures"));
                        serverPrivilegedUserList = SQLHelpers.GetString(reader, "auditUsersList");
                        serverAuditUserCaptureDDL = (Boolean)reader[reader.GetOrdinal("auditUserCaptureDDL")];
                        serverAuditUserCaptureSQL = (Boolean)reader[reader.GetOrdinal("auditUserCaptureSQL")];
                        serverAuditUserCaptureTrans = (Boolean)reader[reader.GetOrdinal("auditUserCaptureTrans")];
                        serverAuditUserAdmin = (Boolean)reader[reader.GetOrdinal("auditUserAdmin")];
                        serverAuditUserSecurity = (Boolean)reader[reader.GetOrdinal("auditUserSecurity")];
                    }
                }
            }
            catch(Exception e)
            {
                MessageBox.Show(e+"");
            }
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
                              break;
                          case DeselectControls.DbFilterEventsFailedOnly:
                              this.UpdateDependentRadioButtons(this._rbUserAuditPassed, checkedValue, this._rbPassed.Checked, deselectOption, deselectControl);
                                this.UpdateDependentRadioButtons(this._rbUserAuditFailed, checkedValue, this._rbFailed.Checked, deselectOption, deselectControl);
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
        private void UpdateDependentCheckboxes(CheckBox dependentControl, bool checkedValue, DeselectOptions deselectOptions, DeselectControls deselectControl, EventHandler checkedChangedHandler = null)
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

        private void ResetSettings_Click(object sender, EventArgs e)
        {
            LoadPropertise(true);
        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
