using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Collections;
using System.Data.SqlClient;
using System.Windows.Forms;
using Idera.SQLcompliance.Application.GUI.Controls;
using Idera.SQLcompliance.Core.Agent;
using Idera.SQLcompliance.Core;
using Idera.SQLcompliance.Core.Collector;
using Idera.SQLcompliance.Core.Licensing;
using Idera.SQLcompliance.Core.Remoting;
using Idera.SQLcompliance.Core.Service;
using Idera.SQLcompliance.Core.Status;
using Idera.SQLcompliance.Core.Event;
using Idera.SQLcompliance.Core.Templates.AuditTemplates;
using Idera.SQLcompliance.Application.GUI.Helper;
using Idera.SQLcompliance.Application.GUI.SQL;
using System.IO;
using Idera.SQLcompliance.Core.Templates;
using Idera.SQLcompliance.Core.Templates.AuditSettings;
using Idera.SQLcompliance.Core.Stats;
using System.Transactions;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Idera.SQLcompliance.Application.GUI.Forms
{
    public partial class Form_RegisterWizard : Form, IDeselectionClient
    {
        private const string COMMA_CHARACTER = ",";
        private bool _dblevelPrivUserEnabled = false;
        private bool _isCustomTemplate = false;
        private bool _templateDBInitializationCompleted = false;
        private bool _templateSvrInitializationCompleted = false;
        private string m_currentExportingDirectory = string.Empty;
        private InstanceTemplate _template;
        // Handles Deselection of UI Controls
        private DeselectionManager _deselectionManager;

        //Fix for 5241
        private bool firstTime = false;
        private int updatedSettings = 0; //Regulation Settings on UI
        private int oldSettings = 0; //Regulation Settings from db

        public enum StartPage
        {
            Server,
            Database,
            RegulationGuidelineInfo
        }

        enum WizardPage
        {
            LicenseLimit,
            ServerName,
            Cluster,
            ExistingDatabase,
            IncompatibleDatabase,
            AgentDeployment,
            RepositoryVersionTooLow,
            AlwaysOnReplicaDetails,
            AgentCredentials,
            AgentTraceDirectory,
            LoadDatabaseError,
            AllDBAdded,
            SelectDatabases,
            Auditing,
            ServerEvents,
            PrivilegedUsers,

            PrivilegedUserEvents,
            DatabaseCategories,
            DMLFilters,
            TrustedUsers,
            RepositoryPermissions,
            RegulationGuidelineInfo,
            Regulation,
            ServerAuditConfig,
            DatabaseAuditConfig,
            SensitiveColumns,
            BeforeAfter,
            PermissionsCheck,
            Summary,
            //start sqlcm 5.6 - 5464
            PrivilegedUsersDatabase,
            PrivilegedUsersDatabaseSettings,
            //end sqlcm 5.6* - 5464
            // v5.6 SQLCM-5373
            ServerTrustedUsers
        }

        enum AuditingType
        {
            Default,
            Custom,
            Regulation
        }

        enum WizardDirection
        {
            Next,
            Previous
        }

        readonly string _titleServer = "SQLcm Configuration Wizard - Add Server";
        readonly string _titleDatabase = "SQLcm Configuration Wizard - Add Databases";
        readonly string _titleAlwaysOn = "SQLcm Configuration Wizard - AlwaysOn Details";
        readonly string _titleAudit = "SQLcm Configuration Wizard - Audit Settings";
        readonly string _titleAuditCustom = "SQLcm Configuration Wizard - Define Custom Audit Settings";
        readonly string _titleRegulation = "SQLcm Configuration Wizard - Apply Regulation";
        private readonly string _titlePermissionsCheck = "SQLcm Configuration Wizard - Permissions Check";
        readonly string _titleSummary = "SQLcm Configuration Wizard - Summary";

        private ServerRecord _server;
        private List<DatabaseRecord> _databases = new List<DatabaseRecord>();
        private List<Regulation> _regulations = new List<Regulation>();
        private Dictionary<int, RegulationSettings> _regulationSettings = new Dictionary<int, RegulationSettings>();
        private Dictionary<string, List<DatabaseObjectRecord>> _scTables = new Dictionary<string, List<DatabaseObjectRecord>>();
        private Dictionary<string, List<DatabaseObjectRecord>> _badTables = new Dictionary<string, List<DatabaseObjectRecord>>();
        private SQLDirect _sqlServer = null;
        private ICollection _tableList = null;
        private Dictionary<string, DatabaseObjectRecord> _tableObjects = null;
        private ListView _lvSCTables = new ListView();
        private Dictionary<string, ListView> _dictLvSCTables = new Dictionary<string, ListView>();

        //For AlwaysOn
        private List<ServerRecord> _replicaServers = new List<ServerRecord>(); //This is for adding the AlwaysOn instances
        private List<DatabaseRecord> _replicaDatabases = new List<DatabaseRecord>();
        private ICollection _dbAVGList = null;
        private List<SecondaryRoleDetails> _secondaryRoleDetailsList = new List<SecondaryRoleDetails>();
        private DatabaseRecord _dbSCTreeSelection;
        private DatabaseRecord _dbBadTreeSelection;
        private Dictionary<string, ListView> _badListViewItems = new Dictionary<string, ListView>();

        private bool _loadedSCTables = false;
        private bool _loadedPrivUsers = false;
        //SQLCM-5581, 5582
        private bool _loadedTrustedUsers = false;
        private bool _isVirtualServer = false;
        private bool _addingDBs = true;
        private bool _licenseExceeded = false;
        private string _lastCheckedName;
        private string _computer;
        private string _instance;
        private int? _instancePort;
        private string _sqlVersionName;
        private bool _isCluster;
        private bool _isHadrEnabled;
        private string _instanceName;
        private string _dbName;
        private bool _loadedBADTables = false;

        private bool _isUserDatabaseLoaded = false;
        private bool _isSystemDatabaseLoaded = false;
        private bool _deploymentSuccessful = false;
        private bool _existingDatabase = false;
        private bool _compatibleSchema = false;
        private bool _convertingNonAudited = false;
        private bool _alreadyDeployedManually = false;
        private bool _alreadyDeployed = false;
        private bool _repositoryComputer = false;
        private ServerRecord _existingServer = null;
        private ServerRecord _existingAuditedServer = null;
        ListView _lvBeforeAfterTables = null;
        private bool _isServerConnection = true;


        private AuditingType _auditType = AuditingType.Default;
        private WizardPage _currentPage = WizardPage.ServerName;
        private StartPage _startPage;

        public bool IsServerConnection
        {
            get { return _isServerConnection; }
            set { _isServerConnection = value; }
        }
        public ServerRecord Server
        {
            get { return _server; }
            set { _server = value; }
        }

        public List<ServerRecord> ReplicaServerList
        {
            get { return _replicaServers; }
            set { _replicaServers = value; }
        }
        public List<DatabaseRecord> DatabaseList
        {
            get { return _databases; }
            set
            {
                _databases = value;
            }
        }

        public List<DatabaseRecord> ReplicaDatabasesList
        {
            get { return _replicaDatabases; }
            set
            {
                _replicaDatabases = value;
            }
        }

        private CheckBox chkUserCaptureDDL = null;
        //start sqlcm 5.6 - 5464
        private CheckBox chkUserCaptureDDLDatabase = null;
        //end sqlcm 5.6 -5464
        private CheckBox chkDBCaptureDDL = null;

        // SQLCM 5563: Disable Logout with Logins for Server settings not properly configured while applying Regulation Guideline at Server Level
        private bool _isNewServer = false;
        public Form_RegisterWizard(StartPage startPage, ServerRecord server)
        {
            _deselectionManager = new DeselectionManager(this);
            InitializeComponent();
            SetToolTipTextAndVisibility();
            if (server != null)
            {
                _server = server;
                _instanceName = server.Instance; //gets used for AlwaysOn instances
                _isNewServer = false;
            }
            else
            {
                _server = new ServerRecord();
                _isNewServer = true;
            }

            _startPage = startPage;
            _sqlServer = new SQLDirect();

            switch (startPage)
            {
                case StartPage.Database:
                    {
                        Globals.isServerNodeSelected = false;
                        //This will load the databases and set the current page
                        LoadDatabaseInfo();
                        pictureBox1.Image = Idera.SQLcompliance.Application.GUI.Properties.Resources.Wizard_DB;
                        break;
                    }
                case StartPage.RegulationGuidelineInfo:
                    {
                        _currentPage = WizardPage.RegulationGuidelineInfo;
                        _auditType = AuditingType.Regulation;
                        LoadRegulations();
                        break;
                    }
                default:
                    {
                        if (LicenseAllowsMoreInstances())
                        {
                            _licenseExceeded = false;
                            _currentPage = WizardPage.ServerName;
                        }
                        else
                        {
                            _licenseExceeded = true;
                            _currentPage = WizardPage.LicenseLimit;
                        }
                        break;
                    }
            }
            ShowPage(WizardDirection.Next);

            chkUserCaptureDDL = new CheckBox();
            chkUserCaptureDDL.Text = "Capture SQL statements for DDL and Security Changes";
            chkUserCaptureDDL.Left = 8;
            chkUserCaptureDDL.Top = 186;
            chkUserCaptureDDL.Width = 300;
            this.grpAuditUserActivity.Controls.Add(chkUserCaptureDDL);

            //start sqlcm 5.6 - 5464
            chkUserCaptureDDLDatabase = new CheckBox();
            chkUserCaptureDDLDatabase.Text = "Capture SQL statements for DDL and Security Changes";
            chkUserCaptureDDLDatabase.Left = 8;
            chkUserCaptureDDLDatabase.Top = 186;
            chkUserCaptureDDLDatabase.Width = 300;
            this.grpAuditUserActivityDatabase.Controls.Add(chkUserCaptureDDLDatabase);
            //end sqlcm 5.6 - 5464


            chkDBCaptureDDL = new CheckBox();
            chkDBCaptureDDL.Text = "Capture SQL statements for DDL and Security Changes";
            chkDBCaptureDDL.Top = 312;
            chkDBCaptureDDL.Left = 17;
            chkDBCaptureDDL.Width = 300;
            this.pageDatabaseCategories.Controls.Add(chkDBCaptureDDL);

            chkUserAuditDDL.CheckedChanged += chkUserAuditDDLSecurity_CheckedChanged;
            chkUserAuditSecurity.CheckedChanged += chkUserAuditDDLSecurity_CheckedChanged;
            chkDBAuditDDL.CheckedChanged += chkDBAuditDDLSecurity_CheckedChanged;
            chkDBAuditSecurity.CheckedChanged += chkDBAuditDDLSecurity_CheckedChanged;
        }


        private void SetToolTipTextAndVisibility()
        {
            toolTipCIS.Text = "The CIS regulation is a consensus based best practice for security configuration guidelines";
            toolTipDISA.Text = "The DISA STIG regulation is a guideline based on recommendations from the Department of Defense (DOD)";
            toolTipHIPAA.Text = "The HIPAA regulation applies to healthcare, medical records, insurance and other medical related business";
            toolTipNERC.Text = "The NERC regulation applies to companies that generate, provide or transmit energy";
            toolTipPCI.Text = "The PCI DSS regulation applies to credit card processors and merchants";
            toolTipSOX.Text = "THE SOX regulation applies to all publicly traded companies";
            toolTipFERPA.Text = "The FERPA regulation applies to children's education records";
            toolTipGDPR.Text = "GDPR is a regulation that protects personally identifiable information for EU Members";
            toolTipCIS.Visible = false;
            toolTipDISA.Visible = false;
            toolTipHIPAA.Visible = false;
            toolTipNERC.Visible = false;
            toolTipPCI.Visible = false;
            toolTipSOX.Visible = false;
            toolTipFERPA.Visible = false;
            toolTipGDPR.Visible = false;
        }

        void chkUserAuditDDLSecurity_CheckedChanged(object sender, EventArgs e)
        {
            if (rbAuditUserSelected.Checked && (chkUserAuditDDL.Checked || chkUserAuditSecurity.Checked) && CoreConstants.AllowCaptureSql)
                chkUserCaptureDDL.Enabled = true;
            else
            {
                chkUserCaptureDDL.Checked = false;
                chkUserCaptureDDL.Enabled = false;
            }
        }

        void chkDBAuditDDLSecurity_CheckedChanged(object sender, EventArgs e)
        {
            if (rbAuditUserSelected.Checked && (chkDBAuditDDL.Checked || chkDBAuditSecurity.Checked) && CoreConstants.AllowCaptureSql)
                chkDBCaptureDDL.Enabled = true;
            else
            {
                chkDBCaptureDDL.Checked = false;
                chkDBCaptureDDL.Enabled = false;
            }
        }

        #region General Wizard Functionality

        private bool LicenseAllowsMoreInstances()
        {
            return LicenseHelper.LicenseAllowsMoreInstances(Globals.Repository.Connection, Globals.SQLcomplianceConfig);
        }

        private void nextButton_Click(object sender, EventArgs e)
        {
            SetNextPage();

            //This tells us the user clicked finish.  Most likely on the Summary Page.  The summary page is the last page
            //so if (_currentPage > WizardPage.Summary would probably work but checking for finish will allow any page to
            //be the "end"
            if (this.nextButton.Text == "Finish")
            {
                this.nextButton.Enabled = false;
                if (!_licenseExceeded)
                {
                    this.DialogResult = DialogResult.OK;
                    if (_currentPage > WizardPage.Summary)
                    {

                        if ((_auditType == AuditingType.Regulation && _isCustomTemplate) || (_auditType == AuditingType.Regulation && checkCustom.Checked))
                        {
                            Form_SaveFileDialog frmSaveDialog = new Form_SaveFileDialog();
                            if (DialogResult.OK == frmSaveDialog.ShowDialog())
                            {
                                Save();
                                if (frmSaveDialog.ApplySaveEnabled)
                                {
                                    string fileName = frmSaveDialog.FileName;
                                    if (Globals.isServerNodeSelected)
                                    {
                                        ExportServerAuditSettingsAction(_server, fileName);
                                    }
                                    else
                                    {
                                        ExportDatabaseAuditSettingsAction(_databases, fileName);
                                    }
                                }
                                this.Close();
                            }
                            else
                            {
                                this.DialogResult = DialogResult.None;
                                nextButton.Enabled = true;
                            }
                        }
                        else
                        {
                            Save();
                            this.Close();
                        }
                    }
                    //SQLCM-6206 Updating audit configuration so that auditxe sp can be created when adding a server.
                    try
                    {
                        AgentManager manager = GUIRemoteObjectsProvider.AgentManager();
                        manager.UpdateAuditConfiguration(_server.Instance);
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

                    ServerRecord.UpdateCountDatabasesAuditingAllObjects(Globals.Repository.Connection, _server.SrvId,
                                           ServerRecord.GetCountAuditingDMLEnabled(Globals.Repository.Connection, _server.SrvId)); //Fix for SQLCM-4967
                }
                else
                {
                    this.DialogResult = DialogResult.Cancel;
                    this.Close();
                }
            }
            else
                ShowPage(WizardDirection.Next);
        }

        private void SetNextPage()
        {
            if (ValidateCurrentPage())
            {
                switch (_currentPage)
                {
                    case WizardPage.ServerName:
                        {
                            _currentPage = WizardPage.Cluster;

                            _existingDatabase = DoesDatabaseExist(out _compatibleSchema);

                            if (_existingDatabase)
                            {
                                if (_compatibleSchema)
                                    _currentPage = WizardPage.ExistingDatabase;
                                else
                                {
                                    _currentPage = WizardPage.IncompatibleDatabase;
                                }
                            }
                            break;
                        }
                    case WizardPage.Cluster:
                        {
                            _currentPage = WizardPage.AgentDeployment;


                            //Jira 5307 - targetversion should be less than repository version
                            this.Cursor = Cursors.WaitCursor;
                            // connect
                            SQLDirect direct = new SQLDirect();
                            String SQLServerName = _instancePort == null ? this.textSQLServer.Text : String.Format("{0},{1}",
                               this.textSQLServer.Text, _instancePort);
                            if (direct.OpenConnection(SQLServerName))
                            {
                                // get version
                                int targetSqlVersion = SQLHelpers.GetSqlVersion(direct.Connection);

                                //  
                                if (targetSqlVersion > Globals.repositorySqlVersion)
                                {
                                    _currentPage = WizardPage.RepositoryVersionTooLow;
                                }
                                direct.CloseConnection();
                            }
                            else
                            {
                                _currentPage = WizardPage.LoadDatabaseError;
                            }
                            this.Cursor = Cursors.Default;
                            break;
                        }
                    case WizardPage.ExistingDatabase:
                        {
                            _currentPage = WizardPage.Cluster;
                            break;
                        }
                    case WizardPage.IncompatibleDatabase:
                        {
                            _currentPage = WizardPage.Cluster;
                            break;
                        }
                    case WizardPage.AgentDeployment:
                        {
                            if (radioButtonDeployNow.Checked)
                                _currentPage = WizardPage.AgentCredentials;

                            //If anything except deploy now is checked, add the instance.
                            // The deploy now call is under TraceDirectory.
                            if (!radioButtonDeployNow.Checked)
                            {
                                Form_AddStatus status = new Form_AddStatus();
                                status.Show(this);
                                AddInstance();
                                //This will load the databases and set the current page
                                status.Message = "Loading Database Info...";
                                LoadDatabaseInfo();
                                status.Close();
                            }
                            break;
                        }
                    case WizardPage.AgentTraceDirectory:
                        {
                            //Deploy the agent and display the Dbs page
                            //We should only be here is the user selected deploy now.
                            //The other calls to AddInstance are under AgentDeployment 
                            //and only add the instance, not deploy it.
                            AddInstance();
                            //This will load the databases and set the current page
                            Form_AddStatus status = new Form_AddStatus();
                            status.Show(this);
                            status.Message = "Loading Database Info...";
                            LoadDatabaseInfo();
                            status.Close();
                            break;
                        }
                    case WizardPage.LoadDatabaseError:
                        {
                            if (_startPage == StartPage.Server)
                            {
                                var result = MessageBox.Show(this, "We are currently unable to reach this server. Auditing may not be available until the server is reachable. Would you like to continue?", Text, MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                                if (result == DialogResult.Yes)
                                {
                                    _isServerConnection = false;
                                    AddInstance();
                                    //This will load the databases and set the current page
                                    //Form_AddStatus status = new Form_AddStatus();
                                    //status.Show(this);
                                    //status.Message = "Loading Database Info...";
                                    //LoadDatabaseInfo();
                                    //status.Close();
                                }
                                else
                                {
                                    this.Close();
                                    _isServerConnection = false;
                                }
                            }
                            _currentPage = WizardPage.Auditing;
                            break;
                        }
                    case WizardPage.AllDBAdded:
                        {
                            _currentPage = WizardPage.Auditing;
                            break;
                        }
                    case WizardPage.Auditing:
                        {
                            switch (_auditType)
                            {
                                case AuditingType.Default:
                                    _currentPage = WizardPage.PermissionsCheck;
                                    break;
                                case AuditingType.Custom:
                                    // Unregister Deselection Events
                                    UnregisterDeselectionManagerEvents();
                                    if (_startPage == StartPage.Database)
                                    {
                                        _currentPage = WizardPage.PrivilegedUsers;
                                        // Register Database Controls
                                        this.RegisterDatabaseEvents();
                                    }
                                    else
                                    {
                                        _currentPage = WizardPage.ServerEvents;
                                        // Register Server Controls
                                        this.RegisterServerEvents();
                                    }
                                    break;
                                case AuditingType.Regulation:
                                    {
                                        if (_startPage == StartPage.Database)
                                        {
                                            Globals.isServerNodeSelected = false;
                                            checkCIS.Visible = false;
                                            pictureCISInfo.Visible = false;
                                        }
                                        else if (_startPage == StartPage.Server)
                                        {
                                            Globals.isServerNodeSelected = true;
                                        }
                                        LoadRegulations();
                                        _currentPage = WizardPage.RegulationGuidelineInfo;
                                        break;
                                    }
                            }
                            break;
                        }
                    case WizardPage.TrustedUsers:
                        {
                            if (_startPage == StartPage.Database)
                                _currentPage = WizardPage.PermissionsCheck;
                            else
                                _currentPage++;
                            break;
                        }
                    case WizardPage.ServerTrustedUsers:// v5.6 SQLCM-5373
                        {
                            if (_startPage == StartPage.Server)
                                _currentPage = WizardPage.RepositoryPermissions;
                            else
                                _currentPage++;
                            break;
                        }
                    case WizardPage.RepositoryPermissions:
                        {
                            _currentPage = WizardPage.PermissionsCheck;
                            break;
                        }
                    case WizardPage.PermissionsCheck:
                        _currentPage = WizardPage.Summary;
                        break;
                    case WizardPage.Regulation:
                        {
                            _templateSvrInitializationCompleted = false;
                            _templateDBInitializationCompleted = false;
                            _isCustomTemplate = false;
                            if (_startPage == StartPage.RegulationGuidelineInfo && checkCustom.Checked)
                            {
                                if (_template.AuditTemplate.ServerLevelConfig != null)
                                {
                                    _currentPage = WizardPage.ServerAuditConfig;
                                    _chkPriviligedUser.Visible = false;
                                }
                                else
                                    _currentPage = WizardPage.DatabaseAuditConfig;
                            }
                            else
                            {
                                if (_startPage == StartPage.RegulationGuidelineInfo && Globals.isServerNodeSelected)
                                {
                                    _currentPage = WizardPage.ServerAuditConfig;
                                    _chkPriviligedUser.Visible = false;
                                }
                                else if (_startPage == StartPage.Server)
                                {
                                    _currentPage = WizardPage.ServerAuditConfig;
                                    _chkPriviligedUser.Visible = false;
                                }
                                else
                                    _currentPage = WizardPage.DatabaseAuditConfig;
                            }
                            break;
                        }
                    case WizardPage.ServerAuditConfig:
                        {
                            if (_auditType == AuditingType.Regulation && checkPriviligedUser.Checked)
                                _currentPage = WizardPage.PrivilegedUsers;
                            else
                            {
                                if (lstPrivilegedUsers.Items.Count > 0)
                                    lstPrivilegedUsers.Items.Clear();
                                _currentPage = WizardPage.DatabaseAuditConfig;
                            }
                            break;
                        }
                    case WizardPage.DatabaseAuditConfig:
                        {
                            if (!Globals.isServerNodeSelected) // Clear in case of if user database level priv user check box unchecked
                            {
                                if (!_chkPriviligedUser.Checked)
                                    if (lstPrivilegedUsers.Items.Count > 0)
                                        lstPrivilegedUsers.Items.Clear();
                            }
                            if (!_chkAuditSensitiveColumn.Checked)
                                if (_dictLvSCTables.Count > 0)
                                    _dictLvSCTables.Clear();

                            if (!_chkAuditBeforeAfter.Checked)
                                if (_badListViewItems.Count > 0)
                                    _badListViewItems.Clear();

                            //start sqlcm 5.6 - 5464 (adding if conditon to check whether to show privileged user roles for database level
                            if (_auditType == AuditingType.Regulation && _chkPriviligedUser.Checked && Globals.isServerNodeSelected)
                            {
                                _currentPage = WizardPage.PrivilegedUsersDatabase;
                                _dblevelPrivUserEnabled = false;
                                /*lstPrivilegedUsersdb.Items.Clear();
                                for (int i = 0; i < lstPrivilegedUsers.Items.Count; i++)
                                {
                                    ListViewItem listViewitem =(ListViewItem) lstPrivilegedUsers.Items[i].Clone();
                                    lstPrivilegedUsersdb.Items.Add(listViewitem);
                                }*/
                            }
                            //end sqlcm 5.6 - 5464

                            else if (_auditType == AuditingType.Regulation && _chkPriviligedUser.Checked)
                            {
                                _currentPage = WizardPage.PrivilegedUsers;
                                _dblevelPrivUserEnabled = true;
                            }
                            else if (_auditType == AuditingType.Regulation && _chkAuditSensitiveColumn.Checked)
                                _currentPage = WizardPage.SensitiveColumns;
                            else if (_auditType == AuditingType.Regulation && _chkAuditBeforeAfter.Checked)
                                _currentPage = WizardPage.BeforeAfter;
                            else
                                _currentPage = WizardPage.PermissionsCheck;
                            break;
                        }
                    case WizardPage.SensitiveColumns:
                        {
                            if (_auditType == AuditingType.Regulation && _chkAuditBeforeAfter.Checked)
                                _currentPage = WizardPage.BeforeAfter;
                            else
                                _currentPage = WizardPage.PermissionsCheck;
                            break;
                        }
                    case WizardPage.PrivilegedUsers:
                        {
                            if (lstPrivilegedUsers.Items.Count == 0)
                            {
                                if (_auditType == AuditingType.Regulation && Globals.isServerNodeSelected && !_dblevelPrivUserEnabled)
                                    _currentPage = WizardPage.DatabaseAuditConfig;
                                else if (_auditType == AuditingType.Regulation && _chkAuditSensitiveColumn.Checked)
                                    _currentPage = WizardPage.SensitiveColumns;
                                else if (_auditType == AuditingType.Regulation && _chkAuditBeforeAfter.Checked)
                                    _currentPage = WizardPage.BeforeAfter;
                                else if (_auditType == AuditingType.Regulation)
                                    _currentPage = WizardPage.PermissionsCheck;
                                else
                                {
                                    if (!_addingDBs)
                                        _currentPage = WizardPage.RepositoryPermissions;
                                    else
                                        _currentPage = WizardPage.DatabaseCategories;
                                }
                            }
                            else
                                _currentPage++;
                            break;
                        }
                    //start sqlcm 5.6 - 5464
                    case WizardPage.PrivilegedUsersDatabase:
                        {

                            if (lstPrivilegedUsersdb.Items.Count == 0)
                            {
                                if (_auditType == AuditingType.Regulation && _chkAuditSensitiveColumn.Checked)
                                    _currentPage = WizardPage.SensitiveColumns;
                                else if (_auditType == AuditingType.Regulation && _chkAuditBeforeAfter.Checked)
                                    _currentPage = WizardPage.BeforeAfter;
                                else if (_auditType == AuditingType.Regulation)
                                    _currentPage = WizardPage.PermissionsCheck;
                                else
                                {
                                    if (!_addingDBs)
                                        _currentPage = WizardPage.RepositoryPermissions;
                                    else
                                        _currentPage = WizardPage.DatabaseCategories;
                                }
                            }
                            else
                                _currentPage = WizardPage.PrivilegedUsersDatabaseSettings;
                            break;
                        }
                    case WizardPage.PrivilegedUsersDatabaseSettings:
                        {

                            if (_auditType == AuditingType.Regulation && _chkAuditSensitiveColumn.Checked)
                                _currentPage = WizardPage.SensitiveColumns;
                            else if (_auditType == AuditingType.Regulation && _chkAuditBeforeAfter.Checked)
                                _currentPage = WizardPage.BeforeAfter;
                            else if (_auditType == AuditingType.Regulation)
                                _currentPage = WizardPage.PermissionsCheck;
                            else
                            {
                                if (!_addingDBs)
                                    _currentPage = WizardPage.RepositoryPermissions;
                                else
                                    _currentPage = WizardPage.DatabaseCategories;
                            }


                            break;
                        }
                    //end sqlcm 5.6 - 5464
                    case WizardPage.PrivilegedUserEvents:
                        {
                            if (_auditType == AuditingType.Regulation && Globals.isServerNodeSelected && !_dblevelPrivUserEnabled)
                                _currentPage = WizardPage.DatabaseAuditConfig;
                            else if (_auditType == AuditingType.Regulation && _chkAuditSensitiveColumn.Checked)
                                _currentPage = WizardPage.SensitiveColumns;
                            else if (_auditType == AuditingType.Regulation && _chkAuditBeforeAfter.Checked)
                                _currentPage = WizardPage.BeforeAfter;
                            else if (_auditType == AuditingType.Regulation)
                                _currentPage = WizardPage.PermissionsCheck;
                            else
                            {
                                if (!_addingDBs)
                                    _currentPage = WizardPage.RepositoryPermissions;
                                else
                                    _currentPage++;
                            }
                            break;
                        }

                    case WizardPage.DatabaseCategories:
                        {
                            if (_startPage == StartPage.Server)// v5.6 SQLCM-5373
                            {
                                if (!_chkAuditDML.Checked && !_chkAuditSelect.Checked)
                                    _currentPage = WizardPage.ServerTrustedUsers;
                                else
                                    _currentPage++;
                            }
                            else
                            {
                                if (!chkDBAuditDML.Checked && !chkDBAuditSELECT.Checked)
                                    _currentPage = WizardPage.TrustedUsers;
                                else
                                    _currentPage++;
                            }
                            break;
                        }
                    case WizardPage.SelectDatabases:
                        {
                            //implement the Availability group db check logic here, if any of the selected DB is part of AVG
                            //then show the details to the user
                            Form_AddStatus status = new Form_AddStatus();
                            status.Show(this);
                            status.Message = "Checking if Databases selected are involved in AlwaysOn Availability Group configuration. Please Wait...";

                            try
                            {
                                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                    String.Format("Fetching the AlwaysOn database details for SQL Server {0}", _server.Instance), ErrorLog.Severity.Informational);
                                this.Cursor = Cursors.WaitCursor;
                                LoadAVGConfigurationDetail();
                                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                    String.Format("Fetching the AlwaysOn database details complete for SQL Server {0}", _server.Instance), ErrorLog.Severity.Informational);
                                status.Close();
                                List<Quadruplet<bool, string, string, string>> avgDetails = new List<Quadruplet<bool, string, string, string>>();
                                if (_dbAVGList != null)
                                {
                                    foreach (RawAVGroup ravg in _dbAVGList)
                                    {
                                        Quadruplet<bool, string, string, string> avgDetail = new Quadruplet<bool, string, string, string>();
                                        avgDetail.Second = ravg.dbName;
                                        avgDetail.Third = ravg.avgName;
                                        avgDetail.Fourth = ravg.replicaServerName;
                                        avgDetails.Add(avgDetail);
                                    }
                                    if (avgDetails.Count > 0)
                                    {
                                        LoadAlwaysOnAvalibilityList(avgDetails);
                                        _currentPage = WizardPage.AlwaysOnReplicaDetails;
                                    }
                                    else
                                        _currentPage = WizardPage.Auditing;

                                }
                                else
                                    _currentPage = WizardPage.Auditing;
                            }
                            catch (Exception e)
                            {
                            }
                            finally
                            {
                                this.Cursor = Cursors.Default;
                            }

                            break;
                        }
                    case WizardPage.AlwaysOnReplicaDetails:
                        {
                            _currentPage = WizardPage.Auditing;
                            break;
                        }
                    default:
                        _currentPage++;
                        break;
                }
            }
        }

        /// <summary>
        /// Register Server Events
        /// </summary>
        private void RegisterServerEvents()
        {
            this._deselectionManager.RegisterCheckbox(this.chkServerAuditLogins, DeselectControls.ServerLogins);
            this._deselectionManager.RegisterCheckbox(this.chkServerAuditLogouts, DeselectControls.ServerLogouts);
            this._deselectionManager.RegisterCheckbox(this.chkServerAuditFailedLogins, DeselectControls.ServerFailedLogins);
            this._deselectionManager.RegisterCheckbox(this.chkServerAuditSecurity, DeselectControls.ServerSecurityChanges);
            this._deselectionManager.RegisterCheckbox(this.chkServerAuditDDL, DeselectControls.ServerDatabaseDefinition);
            this._deselectionManager.RegisterCheckbox(this.chkServerAuditAdmin, DeselectControls.ServerAdministrativeActivities);
            this._deselectionManager.RegisterCheckbox(this.chkServerAccessCheckFilter, DeselectControls.ServerFilterEvents);
            this._deselectionManager.RegisterRadioButton(this.rbServerAuditSuccessfulOnly, DeselectControls.ServerFilterEventsPassOnly);
            this._deselectionManager.RegisterRadioButton(this.rbAuditFailedOnly, DeselectControls.ServerFilterEventsFailedOnly);
            this._deselectionManager.RegisterCheckbox(this.chkServerAuditUDE, DeselectControls.ServerUserDefined);
        }

        /// <summary>
        /// Register Database
        /// </summary>
        private void RegisterDatabaseEvents()
        {
            this._deselectionManager.RegisterCheckbox(this.chkDBAccessCheckFilter, DeselectControls.DbFilterEvents);
            this._deselectionManager.RegisterRadioButton(this.rbDBAuditFailedOnly, DeselectControls.DbFilterEventsFailedOnly);
            this._deselectionManager.RegisterRadioButton(this.rbDBAuditSuccessfulOnly, DeselectControls.DbFilterEventsPassOnly);
            this._deselectionManager.RegisterCheckbox(this.chkDBAuditCaptureSQL, DeselectControls.DbCaptureSqlDmlSelect);
            this._deselectionManager.RegisterCheckbox(this.chkDBAuditCaptureTrans, DeselectControls.DbCaptureSqlTransactionStatus);
            this._deselectionManager.RegisterCheckbox(this.chkDBCaptureDDL, DeselectControls.DbCaptureSqlDdlSecurity);
            this._deselectionManager.RegisterCheckbox(this.chkDBAuditDDL, DeselectControls.DbDatabaseDefinition);
            this._deselectionManager.RegisterCheckbox(this.chkDBAuditSecurity, DeselectControls.DbSecurityChanges);
            this._deselectionManager.RegisterCheckbox(this.chkDBAuditAdmin, DeselectControls.DbAdministrativeActivities);
            this._deselectionManager.RegisterCheckbox(this.chkDBAuditDML, DeselectControls.DbDatabaseModifications);
            this._deselectionManager.RegisterCheckbox(this.chkDBAuditSELECT, DeselectControls.DbDatabaseSelect);
        }

        private void previousButton_Click(object sender, EventArgs e)
        {
            if (permissionsCheck.CheckStatus == PermissionsCheck.Status.InProgress)
                permissionsCheck.StopPermissionsCheck();

            SetPreviousPage();
            ShowPage(WizardDirection.Previous);
        }

        private void SetPreviousPage()
        {
            switch (_currentPage)
            {
                case WizardPage.RegulationGuidelineInfo:
                    {
                        _currentPage = WizardPage.Auditing;
                        break;
                    }
                case WizardPage.Cluster:
                    {
                        if (_existingDatabase)
                        {
                            if (_compatibleSchema)
                                _currentPage = WizardPage.ExistingDatabase;
                            else
                                _currentPage = WizardPage.IncompatibleDatabase;
                        }
                        else
                            _currentPage = WizardPage.ServerName;
                        break;
                    }
                case WizardPage.AgentDeployment:
                    {
                        _currentPage = WizardPage.Cluster;
                        break;
                    }
                case WizardPage.AgentCredentials:
                    {
                        _currentPage = WizardPage.AgentDeployment;
                        break;
                    }
                case WizardPage.RepositoryVersionTooLow:
                    {
                        _currentPage = WizardPage.ServerName;
                        break;
                    }
                case WizardPage.IncompatibleDatabase:
                    {
                        _currentPage = WizardPage.ServerName;
                        break;
                    }
                case WizardPage.ExistingDatabase:
                    {
                        _currentPage = WizardPage.ServerName;
                        break;
                    }
                case WizardPage.Auditing:
                    {
                        if (_dbAVGList != null && _dbAVGList.Count > 0)
                            _currentPage = WizardPage.AlwaysOnReplicaDetails;
                        else
                            _currentPage = WizardPage.SelectDatabases;
                        break;
                    }
                case WizardPage.AlwaysOnReplicaDetails:
                    {
                        _currentPage = WizardPage.SelectDatabases;
                        break;
                    }
                case WizardPage.DatabaseCategories:
                    {
                        if (lstPrivilegedUsers.Items.Count == 0)
                            _currentPage = WizardPage.PrivilegedUsers;
                        else
                            _currentPage = WizardPage.PrivilegedUserEvents;
                        break;
                    }
                case WizardPage.DatabaseAuditConfig:
                    {
                        if ((_startPage == StartPage.Server || _startPage == StartPage.RegulationGuidelineInfo) && Globals.isServerNodeSelected && !_dblevelPrivUserEnabled)
                        {
                            if (checkPriviligedUser.Checked)
                            {
                                if (lstPrivilegedUsers.Items.Count == 0)
                                    _currentPage = WizardPage.PrivilegedUsers;
                                else
                                    _currentPage = WizardPage.PrivilegedUserEvents;
                            }
                            else
                            {
                                if (checkCustom.Checked && _template.AuditTemplate.ServerLevelConfig == null)
                                {
                                    _currentPage = WizardPage.Regulation;
                                }
                                else
                                {
                                    _currentPage = WizardPage.ServerAuditConfig;
                                }
                            }
                        }
                        else
                        {
                            _currentPage = WizardPage.Regulation;
                        }
                        break;
                    }
                case WizardPage.PrivilegedUsers:
                    {
                        switch (_auditType)
                        {
                            case AuditingType.Regulation:
                                if (_dblevelPrivUserEnabled == false)
                                    _currentPage = WizardPage.ServerAuditConfig;
                                else
                                {
                                    _currentPage = WizardPage.DatabaseAuditConfig;
                                    _dblevelPrivUserEnabled = false;
                                }

                                break;
                            default:
                                switch (_startPage)
                                {
                                    case StartPage.Server:
                                        _currentPage = WizardPage.ServerEvents;
                                        break;

                                    case StartPage.Database:
                                        _currentPage = WizardPage.Auditing;
                                        break;
                                }
                                break;
                        }
                        break;
                    }
                //start sqlcm 5.6 - 5464
                case WizardPage.PrivilegedUsersDatabase:
                    {
                        _currentPage = WizardPage.DatabaseAuditConfig;
                        break;
                    }
                case WizardPage.PrivilegedUsersDatabaseSettings:
                    {
                        _currentPage = WizardPage.PrivilegedUsersDatabase;
                        break;
                    }
                //end sqlcm 5.6 - 5464
                case WizardPage.SensitiveColumns:
                    {
                        if (_chkPriviligedUser.Checked)
                        {
                            //start sqlcm 5.6 - 5464
                            if (!Globals.isServerNodeSelected)
                            {
                                if (lstPrivilegedUsers.Items.Count == 0)
                                    _currentPage = WizardPage.PrivilegedUsers;
                                else
                                    _currentPage = WizardPage.PrivilegedUserEvents;
                            }
                            else if (lstPrivilegedUsersdb.Items.Count == 0)
                            {
                                _currentPage = WizardPage.PrivilegedUsersDatabase;
                                break;
                            }
                            else
                            {
                                _currentPage = WizardPage.PrivilegedUsersDatabaseSettings;
                                break;
                            }
                            //end sqlcm 5.6 - 5464
                        }
                        else
                        {
                            _currentPage = WizardPage.DatabaseAuditConfig;
                            _dblevelPrivUserEnabled = false;
                        }
                        break;
                    }
                case WizardPage.TrustedUsers:
                    {
                        if (!chkDBAuditDML.Checked && !chkDBAuditSELECT.Checked)
                            _currentPage = WizardPage.DatabaseCategories;
                        else
                            _currentPage--;
                        break;
                    }
                case WizardPage.ServerTrustedUsers:// v5.6 SQLCM-5373
                    {
                        if (!_chkAuditDML.Checked && !_chkAuditSelect.Checked)
                            _currentPage = WizardPage.DatabaseCategories;
                        else
                            _currentPage--;
                        break;
                    }
                case WizardPage.BeforeAfter:
                    {
                        if (_chkAuditSensitiveColumn.Checked)
                            _currentPage = WizardPage.SensitiveColumns;
                        else if (_chkPriviligedUser.Checked)
                        {
                            //start sqlcm 5.6 - 5464
                            if (!Globals.isServerNodeSelected)
                            {
                                if (lstPrivilegedUsers.Items.Count == 0)
                                    _currentPage = WizardPage.PrivilegedUsers;
                                else
                                    _currentPage = WizardPage.PrivilegedUserEvents;
                            }
                            else if (lstPrivilegedUsersdb.Items.Count == 0)
                            {
                                _currentPage = WizardPage.PrivilegedUsersDatabase;
                                break;
                            }
                            else
                            {
                                _currentPage = WizardPage.PrivilegedUsersDatabaseSettings;
                                break;
                            }
                            //end sqlcm 5.6 - 5464
                        }
                        else
                        {
                            _currentPage = WizardPage.DatabaseAuditConfig;
                            _dblevelPrivUserEnabled = false;
                        }
                        break;
                    }
                case WizardPage.PermissionsCheck:
                    {
                        switch (_auditType)
                        {
                            case AuditingType.Default:
                                {
                                    _currentPage = WizardPage.Auditing;
                                    break;
                                }
                            case AuditingType.Custom:
                                {
                                    if (_startPage == StartPage.Database)
                                        _currentPage = WizardPage.TrustedUsers;
                                    else
                                        _currentPage = WizardPage.RepositoryPermissions;
                                    break;
                                }
                            case AuditingType.Regulation:
                                {
                                    if (_chkAuditBeforeAfter.Checked)
                                        _currentPage = WizardPage.BeforeAfter;
                                    else if (_chkAuditSensitiveColumn.Checked)
                                        _currentPage = WizardPage.SensitiveColumns;
                                    else if (_chkPriviligedUser.Checked)
                                    {
                                        //start sqlcm 5.6 - 5464
                                        if (!Globals.isServerNodeSelected)
                                        {
                                            if (lstPrivilegedUsers.Items.Count == 0)
                                                _currentPage = WizardPage.PrivilegedUsers;
                                            else
                                                _currentPage = WizardPage.PrivilegedUserEvents;
                                        }
                                        else if (lstPrivilegedUsersdb.Items.Count == 0)
                                        {
                                            _currentPage = WizardPage.PrivilegedUsersDatabase;

                                        }
                                        else
                                        {
                                            _currentPage = WizardPage.PrivilegedUsersDatabaseSettings;
                                        }
                                        //end sqlcm 5.6 - 5464
                                    }
                                    else
                                    {
                                        _currentPage = WizardPage.DatabaseAuditConfig;
                                        _dblevelPrivUserEnabled = false;
                                    }
                                    break;
                                }
                        }
                        break;
                    }
                case WizardPage.Summary:
                    {
                        _currentPage = WizardPage.PermissionsCheck;
                        break;
                    }
                case WizardPage.RepositoryPermissions:
                    {
                        if (!_addingDBs)
                        {
                            //start sqlcm 5.6 - 5464
                            if (!Globals.isServerNodeSelected)
                            {
                                if (lstPrivilegedUsers.Items.Count == 0)
                                    _currentPage = WizardPage.PrivilegedUsers;
                                else
                                    _currentPage = WizardPage.PrivilegedUserEvents;
                            }
                            else if (lstPrivilegedUsersdb.Items.Count == 0)
                            {
                                _currentPage = WizardPage.PrivilegedUsersDatabase;
                            }
                            else
                            {
                                _currentPage = WizardPage.PrivilegedUsersDatabaseSettings;
                            }
                            //end sqlcm 5.6 - 5464
                        }
                        else if (_startPage == StartPage.Server)
                            _currentPage = WizardPage.ServerTrustedUsers;
                        else
                            _currentPage--;
                        break;
                    }
                default:
                    _currentPage--;
                    break;
            }
        }

        private bool ValidateCurrentPage()
        {
            switch (_currentPage)
            {
                case WizardPage.ServerName:
                    {
                        //only check the server once.
                        if (textSQLServer.Text != _lastCheckedName)
                        {
                            if (!ValidateServerName())
                            {
                                ErrorMessage.Show(this.Text, UIConstants.Error_InvalidServerName);
                                return false;
                            }

                            // see if server is already registered
                            // compare against existing registered instances
                            string instanceServer;

                            if (String.IsNullOrEmpty(_computer))
                                instanceServer = Dns.GetHostName().ToUpper();
                            else
                                instanceServer = _computer;

                            _convertingNonAudited = false;
                            _alreadyDeployedManually = false;
                            _alreadyDeployed = _repositoryComputer;

                            ICollection serverList;
                            serverList = ServerRecord.GetServers(Globals.Repository.Connection, false);

                            if ((serverList != null) && (serverList.Count != 0))
                            {
                                foreach (ServerRecord config in serverList)
                                {
                                    if (config.IsAuditedServer)
                                    {
                                        if (config.Instance.ToUpper() == textSQLServer.Text.ToUpper())
                                        {
                                            ErrorMessage.Show(this.Text, UIConstants.Error_ServerAlreadyRegistered);
                                            return false;
                                        }

                                        // some possible states depend on state of already
                                        // audited instances on same computer				      
                                        if (config.InstanceServer.ToUpper() == instanceServer.ToUpper())
                                        {
                                            if (_existingAuditedServer == null)
                                            {
                                                _existingAuditedServer = config;
                                            }
                                            if (config.IsDeployed)
                                            {
                                                _alreadyDeployed = true;
                                                _alreadyDeployedManually = config.IsDeployedManually;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (config.Instance.ToUpper() == textSQLServer.Text.ToUpper())
                                        {
                                            _existingServer = config;
                                            _convertingNonAudited = true;
                                        }
                                    }
                                }
                            }

                            // since we are here we have a valid name dont check again until it changes
                            _lastCheckedName = textSQLServer.Text;
                            _isVirtualServer = false;
                            checkVirtualServer.Checked = false;
                            radioButtonDeployNow.Checked = true;
                        }
                        return true;
                    }
                case WizardPage.Cluster:
                    {
                        _isVirtualServer = checkVirtualServer.Checked;
                        return true;
                    }
                case WizardPage.AgentCredentials:
                    {
                        if (radioButtonDeployNow.Checked)
                        {
                            if (!ValidateAccountName())
                            {
                                ErrorMessage.Show(this.Text, UIConstants.Error_InvalidServiceAccountName);
                                return false;
                            }

                            if (textServicePassword.Text != textServicePasswordConfirm.Text)
                            {
                                ErrorMessage.Show(this.Text, UIConstants.Error_MismatchedPasswords);
                                return false;
                            }

                            try
                            {
                                // capture exception if installutil not deployed
                                if (InstallUtil.VerifyPassword(textServiceAccount.Text, textServicePassword.Text) != 0)
                                {
                                    ErrorMessage.Show(this.Text, UIConstants.Error_InvalidDomainCredentials);
                                    return false;
                                }
                            }
                            catch (Exception ex)
                            {
                                ErrorMessage.Show(this.Text, String.Format(UIConstants.Error_NoInstallUtilLib, ex.Message));
                                return false;
                            }
                        }
                        return true;
                    }
                case WizardPage.AgentTraceDirectory:
                    {
                        if (radioButtonDeployNow.Checked)
                        {
                            if (radioSpecifyTrace.Checked)
                            {
                                if (!UIUtils.ValidatePath(txtTraceDirectory.Text))
                                {
                                    ErrorMessage.Show(this.Text, UIConstants.Error_InvalidTraceDirectory);
                                    return false;
                                }
                            }
                        }
                        return true;
                    }
                case WizardPage.PrivilegedUserEvents:
                    {
                        if (lstPrivilegedUsers.Items.Count > 0 && rbAuditUserSelected.Checked)
                        {
                            // make sure something checked
                            if (!chkUserAuditLogins.Checked &&
                                !chkUserAuditFailedLogins.Checked &&
                                !chkUserAuditSecurity.Checked &&
                                !chkUserAuditDDL.Checked &&
                                !chkUserAuditAdmin.Checked &&
                                !chkUserAuditDML.Checked &&
                                !chkUserAuditSELECT.Checked &&
                                !chkUserAuditUDE.Checked)
                            {
                                ErrorMessage.Show(this.Text, UIConstants.Error_MustSelectOneAuditUserOption);
                                return false;
                            }
                        }
                        return true;
                    }
                case WizardPage.RegulationGuidelineInfo:
                    {
                        if (_startPage == StartPage.RegulationGuidelineInfo && !Globals.isServerNodeSelected)
                        {
                            checkCIS.Visible = false;
                            pictureCISInfo.Visible = false;
                        }
                        return true;
                    }
                case WizardPage.Regulation:
                    {
                        if (checkCustom.Checked)
                        {
                            if (_tbFile.Text == String.Empty)
                            {
                                MessageBox.Show("Please select a template file for Custom Template Guideline.", "Error");
                                return false;
                            }
                            _template = new InstanceTemplate();
                            if (!_template.Load(_tbFile.Text))
                            {
                                MessageBox.Show(this, "The uploaded XML is not in the correct format. Please review the help files for more details.", "Error");
                                _template = null;
                                return false;
                            }
                            else if (_template.AuditTemplate == null)
                            {
                                MessageBox.Show(this, "The selected file does not contain Audit Settings.", "Error");
                                _template = null;
                                return false;
                            }
                            else if (Globals.isServerNodeSelected && _template.AuditTemplate.ServerLevelConfig == null)
                            {
                                MessageBox.Show(this, "The selected file does not contain Server Audit Settings.", "Error");
                                _template = null;
                                return false;
                            }
                            else if (!Globals.isServerNodeSelected && _template.AuditTemplate.ServerLevelConfig != null)
                            {
                                MessageBox.Show(this, "The selected file contains Server Audit Settings.", "Error");
                                _template = null;
                                return false;
                            }
                        }
                        return true;
                    }
                case WizardPage.DatabaseAuditConfig:
                    {
                        if (!_chkAuditDDL.Checked &&
                                !_chkAuditDML.Checked &&
                                !_chkAuditSecurityChanges.Checked &&
                                !_chkAuditAdminActivity.Checked &&
                                !_chkAuditSelect.Checked)
                        {
                            ErrorMessage.Show(this.Text, UIConstants.Error_MustSelectOneAuditOption);
                            return false;
                        }
                        else if (_chkAuditBeforeAfter.Checked && !_chkAuditDML.Checked)
                        {
                            MessageBox.Show("DML must be enabled in order to gather Before-After Data. Please select DML or deselect Before-After Data.", "Information");
                            return false;
                        }
                        return true;
                    }
                default:
                    return true;
            }
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            if (permissionsCheck.CheckStatus == PermissionsCheck.Status.InProgress)
                permissionsCheck.StopPermissionsCheck();

            //If we have added a server, set it to ok.
            if (_currentPage > WizardPage.AgentTraceDirectory &&
                _startPage == StartPage.Server)
            {
                var messageBoxResult = MessageBox.Show(UIConstants.Warning_ServerAlreadyAdded, UIConstants.Title_ServerAlreadyAdded, MessageBoxButtons.YesNoCancel);

                switch (messageBoxResult)
                {
                    case DialogResult.Yes:
                        Helper.Server.RemoveServer(Server);
                        DialogResult = DialogResult.Cancel;
                        Close();
                        break;
                    case DialogResult.No:
                        DialogResult = DialogResult.OK;
                        Close();
                        break;
                    case DialogResult.Cancel:
                        DialogResult = DialogResult.None;
                        break;
                }
            }
        }

        private void ShowPage(WizardDirection direction)
        {
            tabControl.Tabs[(int)_currentPage].Selected = true;
            UpdateView(direction);
        }

        void UpdateView(WizardDirection direction)
        {
            switch (_currentPage)
            {
                case WizardPage.LicenseLimit:
                    {
                        this.previousButton.Enabled = false;
                        this.nextButton.Enabled = true;
                        this.nextButton.Text = "Finish";
                        this.cancelButton.Enabled = true;
                        this.Text = _titleServer;
                        this.labelTitle.Text = "License Limit Reached";
                        this.labelDescription.Text = "Maximum number of licensed instances reached.";
                        linkMoreInfo.Visible = false;
                        break;
                    }
                case WizardPage.ServerName:
                    {
                        if (String.IsNullOrEmpty(textSQLServer.Text))
                            this.nextButton.Enabled = false;
                        else
                            this.nextButton.Enabled = true;
                        this.previousButton.Enabled = false;
                        this.cancelButton.Enabled = true;
                        this.Text = _titleServer;
                        this.labelTitle.Text = "Specify SQL Server";
                        this.labelDescription.Text = "Specify the SQL Server to register with SQL Compliance Manager. Once a SQL Server is registered, you can begin auditing database activity on the server.";
                        linkMoreInfo.Visible = false;
                        textSQLServer.Focus();
                        break;
                    }
                case WizardPage.Cluster:
                    {
                        this.previousButton.Enabled = true;
                        this.nextButton.Enabled = true;
                        this.nextButton.Focus();
                        this.cancelButton.Enabled = true;
                        this.Text = _titleServer;
                        this.labelTitle.Text = "SQL Server Cluster";
                        this.labelDescription.Text = "Specify whether this is a virtual SQL Server hosted on a cluster.";
                        linkMoreInfo.Visible = false;
                        break;
                    }
                case WizardPage.ExistingDatabase:
                    {
                        this.previousButton.Enabled = true;
                        this.nextButton.Enabled = true;
                        this.cancelButton.Enabled = true;
                        this.Text = _titleServer;
                        this.labelTitle.Text = "Existing Audit Data";
                        this.labelDescription.Text = "Audit data for this SQL Server instance already exists.";
                        linkMoreInfo.Visible = false;
                        break;
                    }
                case WizardPage.IncompatibleDatabase:
                    {
                        this.previousButton.Enabled = true;
                        this.nextButton.Enabled = true;
                        this.nextButton.Text = "Finish";
                        this.cancelButton.Enabled = true;
                        this.Text = _titleServer;
                        this.labelTitle.Text = "Existing Incompatible Database";
                        this.labelDescription.Text = "A database with an unsupported schema already exists for this instance.";
                        linkMoreInfo.Visible = false;
                        break;
                    }
                case WizardPage.AgentDeployment:
                    {
                        this.previousButton.Enabled = true;
                        this.nextButton.Enabled = true;
                        this.cancelButton.Enabled = true;
                        this.Text = _titleServer;
                        this.labelTitle.Text = "SQLcompliance Agent Deployment";
                        this.labelDescription.Text = "Specify the deployment option for this instance's agent.";
                        linkMoreInfo.Visible = false;

                        if (_alreadyDeployed)
                        {
                            radioButtonAlreadyDeployed.Checked = true;
                            radioButtonAlreadyDeployed.Visible = true;
                            radioButtonDeployLater.Enabled = false;
                            radioButtonDeployNow.Enabled = false;
                            radioButtonDeployManually.Enabled = false;
                        }
                        else if (_isVirtualServer)
                        {
                            radioButtonAlreadyDeployed.Visible = false;
                            radioButtonDeployLater.Enabled = false;
                            radioButtonDeployNow.Enabled = false;
                            radioButtonDeployManually.Enabled = true;
                            radioButtonDeployManually.Checked = true;
                        }
                        else
                        {
                            radioButtonAlreadyDeployed.Visible = false;
                            radioButtonDeployLater.Enabled = true;
                            radioButtonDeployNow.Enabled = true;
                            radioButtonDeployManually.Enabled = true;
                        }

                        if (radioButtonDeployNow.Checked)
                            radioButtonDeployNow.Focus();
                        else if (radioButtonDeployLater.Checked)
                            radioButtonDeployLater.Focus();
                        else if (radioButtonDeployManually.Checked)
                            radioButtonDeployManually.Focus();
                        else if (radioButtonAlreadyDeployed.Checked && radioButtonAlreadyDeployed.Visible)
                            radioButtonAlreadyDeployed.Focus();
                        break;
                    }
                case WizardPage.RepositoryVersionTooLow:
                    {
                        this.previousButton.Enabled = true;
                        this.nextButton.Enabled = false;
                        this.cancelButton.Enabled = true;
                        this.Text = _titleServer;
                        this.labelTitle.Text = "Unsupported SQL Server Version";
                        this.labelDescription.Text = "Current configuration does not allow for auditing the select SQL Server Instance.";
                        break;
                    }
                case WizardPage.AgentCredentials:
                    {
                        this.previousButton.Enabled = true;
                        this.nextButton.Enabled = true;
                        this.cancelButton.Enabled = true;
                        this.Text = _titleServer;
                        textServiceAccount.Focus();
                        this.labelTitle.Text = "SQLcompliance Agent Service Account";
                        this.labelDescription.Text = "Specify the service options.  This account needs to be given SQL Server Administrator privileges on the registerd SQL Server.";
                        linkMoreInfo.Visible = false;
                        break;
                    }
                case WizardPage.AgentTraceDirectory:
                    {
                        this.previousButton.Enabled = true;
                        this.nextButton.Enabled = true;
                        this.cancelButton.Enabled = true;
                        this.Text = _titleServer;
                        this.labelTitle.Text = "SQLcompliance Agent Trace Directory";
                        this.labelDescription.Text = "Specify directory for temporary storage of audit data.";
                        linkMoreInfo.Visible = false;
                        break;
                    }
                case WizardPage.LoadDatabaseError:
                    {
                        this.previousButton.Enabled = false;
                        this.nextButton.Enabled = true;

                        //if we are adding DBs and the load fails, there is nothing left to do.
                        //if we just added an instance, we can still configure the audit settings.
                        if (_startPage == StartPage.Database)
                            this.nextButton.Text = "Finish";
                        this.cancelButton.Enabled = true;
                        this.Text = _titleDatabase;
                        this.labelTitle.Text = "Database Load Error";
                        this.labelDescription.Text = "An error occurred loading the list of databases.";
                        linkMoreInfo.Visible = false;
                        break;
                    }
                case WizardPage.AllDBAdded:
                    {
                        this.previousButton.Enabled = false;
                        this.nextButton.Enabled = true;
                        this.nextButton.Text = "Finish";
                        this.cancelButton.Enabled = true;
                        this.Text = _titleDatabase;
                        this.labelTitle.Text = "Add Databases?";
                        this.labelDescription.Text = "Would like to add some databases?";
                        linkMoreInfo.Visible = false;
                        break;
                    }
                case WizardPage.SelectDatabases:
                    {
                        this.previousButton.Enabled = false;

                        if (checkAuditDatabases.Checked && listDatabases.CheckedItems.Count == 0)
                            this.nextButton.Enabled = false;
                        else
                            this.nextButton.Enabled = true;
                        this.cancelButton.Enabled = true;
                        this.Text = _titleDatabase;
                        this.labelTitle.Text = "Select Databases";
                        this.labelDescription.Text = "Select the databases you want to audit.  SQL Compliance Manager will collect audit data for the selected databases.";
                        pictureBox1.Image = Idera.SQLcompliance.Application.GUI.Properties.Resources.Wizard_DB;
                        linkMoreInfo.Visible = false;

                        if (_startPage == StartPage.Database)
                        {
                            checkAuditDatabases.Checked = true;
                            checkAuditDatabases.Enabled = false;
                            checkAuditDatabases.Visible = false;
                            groupDatabases.Enabled = true;
                        }
                        else
                        {
                            checkAuditDatabases.Enabled = true;
                            checkAuditDatabases.Visible = true;
                            checkAuditDatabases_CheckedChanged(null, null);
                        }
                        break;
                    }
                case WizardPage.AlwaysOnReplicaDetails:
                    {
                        this.previousButton.Enabled = true;
                        this.nextButton.Enabled = true;
                        this.cancelButton.Enabled = true;
                        this.Text = _titleAlwaysOn;
                        this.labelTitle.Text = "AlwaysOn Availability Group Details";
                        this.labelDescription.Text = "Showing the Databases that are involved in AlwaysOn Availability Group configuration.";
                        pictureBox1.Image = Idera.SQLcompliance.Application.GUI.Properties.Resources.Wizard_DB;
                        linkMoreInfo.Visible = false;
                        break;
                    }
                case WizardPage.Auditing:
                    {
                        this.UnregisterDeselectionManagerEvents();
                        this.previousButton.Enabled = _isUserDatabaseLoaded;
                        this.nextButton.Enabled = true;
                        this.nextButton.Text = "Next";
                        this.cancelButton.Enabled = true;
                        this.Text = _titleAudit;
                        this.labelTitle.Text = "Audit Collection Level";
                        this.labelDescription.Text = "Select the audit collection level you want to use for the newly audited database. The collection level affects the amount of event data collected for database activities.";
                        linkMoreInfo.Visible = false;

                        if (listDatabases.CheckedItems.Count == 0)
                        {
                            radioRegulation.Enabled = false;
                            radioRegulation.Visible = false;
                        }
                        else
                        {
                            radioRegulation.Enabled = true;
                            radioRegulation.Visible = true;
                        }
                        break;
                    }
                case WizardPage.ServerEvents:
                    {
                        this.previousButton.Enabled = true;
                        this.nextButton.Enabled = true;
                        this.cancelButton.Enabled = true;
                        this.Text = _titleAuditCustom;
                        this.labelTitle.Text = "Server Audit Settings";
                        this.labelDescription.Text = "Select the type of audit data you want to collect for this SQL Server instance.";
                        pictureBox1.Image = Idera.SQLcompliance.Application.GUI.Properties.Resources.Wizard_Server;
                        linkMoreInfo.Visible = true;
                        linkMoreInfo.Text = "Learn how to optomize performance with audit settings.";
                        break;
                    }
                case WizardPage.PrivilegedUsers:
                    {
                        this.previousButton.Enabled = true;
                        this.nextButton.Enabled = true;
                        this.cancelButton.Enabled = true;
                        linkMoreInfo.Visible = false;

                        if (_auditType == AuditingType.Regulation)
                            this.Text = _titleRegulation;
                        else
                            this.Text = _titleAuditCustom;

                        this.labelTitle.Text = "Privileged Users";
                        this.labelDescription.Text = "Select users whose activity you want audited regardless of other audit settings like included databases.  Select server logins and roles to specify privileged users.";

                        if (direction == WizardDirection.Next && (Globals.isServerNodeSelected || _startPage == StartPage.Database))
                        {
                            InitPrivilegedUsers();
                        }
                        else if (direction == WizardDirection.Next)
                        {
                            if (_databases.Count > 0)
                            {
                                LoadPrivilegedUsers(_databases[0]);
                            }
                        }

                        break;
                    }
                case WizardPage.PrivilegedUsersDatabase:
                    {
                        this.previousButton.Enabled = true;
                        this.nextButton.Enabled = true;
                        this.cancelButton.Enabled = true;
                        linkMoreInfo.Visible = false;

                        if (_auditType == AuditingType.Regulation)
                            this.Text = _titleRegulation;
                        else
                            this.Text = _titleAuditCustom;

                        this.labelTitle.Text = "Privileged Users at Database level";
                        this.labelDescription.Text = "Select users whose activity you want audited regardless of other audit settings like included databases.  Select server logins and roles to specify database privileged users.";

                        if (direction == WizardDirection.Next && (Globals.isServerNodeSelected || _startPage == StartPage.Database))
                        {
                            InitPrivilegedUsers();
                        }
                        else if (direction == WizardDirection.Next)
                        {
                            if (_databases.Count > 0)
                            {
                                LoadPrivilegedUsers(_databases[0]);
                            }
                        }

                        break;
                    }

                case WizardPage.PrivilegedUserEvents:
                    {
                        this.previousButton.Enabled = true;
                        this.nextButton.Enabled = true;
                        this.cancelButton.Enabled = true;

                        if (_auditType == AuditingType.Regulation)
                            this.Text = _titleRegulation;
                        else
                            this.Text = _titleAuditCustom;

                        this.labelTitle.Text = "Privileged User Audited Activity";
                        this.labelDescription.Text = "Select which activities to audit for privileged users.";
                        linkMoreInfo.Visible = false;
                        break;
                    }
                //start sqlcm 5.6 - 5464
                case WizardPage.PrivilegedUsersDatabaseSettings:
                    {
                        this.previousButton.Enabled = true;
                        this.nextButton.Enabled = true;
                        this.cancelButton.Enabled = true;

                        if (_auditType == AuditingType.Regulation)
                            this.Text = _titleRegulation;
                        else
                            this.Text = _titleAuditCustom;

                        this.labelTitle.Text = "Database Level Privileged User Audited Activity";
                        this.labelDescription.Text = "Select which activities to audit for privileged users at database level.";
                        linkMoreInfo.Visible = false;
                        break;
                    }
                //end sqlcm 5.6 - 5464
                case WizardPage.DatabaseCategories:
                    {
                        this.previousButton.Enabled = true;
                        this.nextButton.Enabled = true;
                        this.cancelButton.Enabled = true;
                        this.Text = _titleAuditCustom;
                        this.labelTitle.Text = "Database Audit Settings";
                        this.labelDescription.Text = "Specify the type of audit data you want to collect on the selected databases.";
                        pictureBox1.Image = Idera.SQLcompliance.Application.GUI.Properties.Resources.Wizard_DB;
                        linkMoreInfo.Visible = true;
                        linkMoreInfo.Text = "Learn how to optomize performance with audit settings.";
                        break;
                    }
                case WizardPage.DMLFilters:
                    {
                        this.previousButton.Enabled = true;
                        this.nextButton.Enabled = true;
                        this.cancelButton.Enabled = true;
                        this.Text = _titleAuditCustom;
                        this.labelTitle.Text = "DML and SELECT Audit Filters";
                        this.labelDescription.Text = "Select the database objects you want to audit for DML and SELECT activities.";
                        linkMoreInfo.Visible = false;
                        SetFilterState();
                        break;
                    }
                case WizardPage.TrustedUsers:
                    {
                        this.previousButton.Enabled = true;
                        this.nextButton.Enabled = true;
                        this.cancelButton.Enabled = true;
                        this.nextButton.Text = "Next";
                        this.Text = _titleAuditCustom;
                        this.labelTitle.Text = "Trusted Users";
                        this.labelDescription.Text = "Select users whose activites you never want collected, regardless of other audit settings.";
                        linkMoreInfo.Visible = false;
                        //SQLCM-5581, 5582
                        if (direction == WizardDirection.Next && (Globals.isServerNodeSelected || _startPage == StartPage.Database))
                        {
                            InitTrustedUsers();
                        }
                        break;
                    }
                case WizardPage.ServerTrustedUsers:// v5.6 SQLCM-5373
                    {
                        this.previousButton.Enabled = true;
                        this.nextButton.Enabled = true;
                        this.cancelButton.Enabled = true;
                        this.nextButton.Text = "Next";
                        this.Text = _titleAuditCustom;
                        this.labelTitle.Text = "Trusted Users";
                        this.labelDescription.Text = "Select users whose activites you never want collected, regardless of other audit settings.";
                        linkMoreInfo.Visible = false;
                        break;
                    }
                case WizardPage.RepositoryPermissions:
                    {
                        this.previousButton.Enabled = true;
                        this.nextButton.Enabled = true;
                        this.nextButton.Text = "Next";
                        this.cancelButton.Enabled = true;
                        this.Text = _titleAuditCustom;
                        this.labelTitle.Text = "Default Permissions";
                        this.labelDescription.Text = "Select the default level of access to audit data for this SQL Server instance.";
                        linkMoreInfo.Visible = false;
                        break;
                    }
                case WizardPage.Regulation:
                    {
                        // Unregister UI Events
                        UnregisterDeselectionManagerEvents();
                        _templateSvrInitializationCompleted = false;
                        this.previousButton.Enabled = true;
                        this.cancelButton.Enabled = true;
                        this.Text = _titleRegulation;
                        this.labelTitle.Text = "";
                        this.labelDescription.Text = "Select the regulation(s) you want to apply to the audited databases.";
                        pictureBox1.Image = Idera.SQLcompliance.Application.GUI.Properties.Resources.Wizard_Server;
                        linkMoreInfo.Visible = false;
                        // Grey Regulation Options
                        //this.GreyRegulationOptions();

                        //Fix for 5241
                        //Load Regulation Settings only once
                        if (!firstTime)
                        {
                            LoadRegulationSettings();
                        }
                        firstTime = true;

                        UpdateNext();
                        break;
                    }
                case WizardPage.RegulationGuidelineInfo:
                    {
                        if (_startPage == StartPage.RegulationGuidelineInfo)
                        {
                            this.previousButton.Enabled = false;
                            this.nextButton.Enabled = true;

                        }
                        else
                        {
                            this.previousButton.Enabled = true;
                            this.nextButton.Enabled = true;
                        }
                        this.cancelButton.Enabled = true;
                        this.Text = _titleRegulation;
                        this.labelTitle.Text = "";
                        this.labelDescription.Text = "What you need to configure your audit settings.";
                        //SetGuidelineInfo();
                        linkMoreInfo.Visible = false;
                        break;
                    }
                case WizardPage.ServerAuditConfig:
                    {
                        this.previousButton.Enabled = true;
                        this.nextButton.Enabled = true;
                        this.nextButton.Text = "Next";
                        this.cancelButton.Enabled = true;
                        this.Text = _titleRegulation;
                        this.labelTitle.Text = "";
                        this.labelDescription.Text = "Select which server events to automatically collect.";
                        //SetGuidelineInfo();
                        linkMoreInfo.Visible = false;
                        if (!_templateSvrInitializationCompleted)
                        {
                            InitializeServerAuditSettings();
                        }
                        break;
                    }
                case WizardPage.DatabaseAuditConfig:
                    {
                        this.previousButton.Enabled = true;
                        this.nextButton.Enabled = true;
                        this.nextButton.Text = "Next";
                        this.cancelButton.Enabled = true;
                        this.Text = _titleRegulation;
                        this.labelTitle.Text = "";
                        this.labelDescription.Text = "Select which database events to automatically collect.";
                        //SetGuidelineInfo();
                        linkMoreInfo.Visible = false;
                        InitializeDBAuditSettings();

                        break;
                    }
                case WizardPage.SensitiveColumns:
                    {
                        this.previousButton.Enabled = true;
                        this.nextButton.Enabled = true;
                        this.nextButton.Text = "Next";
                        this.cancelButton.Enabled = true;
                        this.Text = _titleRegulation;
                        this.labelTitle.Text = "Sensitive Tables and Columns Access";
                        this.labelDescription.Text = "Select each database table(s) you want to audit for sensitive column access. All table columns will be audited.";
                        linkMoreInfo.Visible = false;

                        if (!_server.IsDeployed)
                        {
                            panelSCStatus.BringToFront();
                            panelSCStatus.Visible = true;
                            labelSCStatus.Text = CoreConstants.Feature_SensitiveColumnNotAvailableVersionUnknown;
                        }
                        else if (!SupportsSensitiveColumns())
                        {
                            panelSCStatus.BringToFront();
                            panelSCStatus.Visible = true;
                            labelSCStatus.Text = CoreConstants.Feature_SensitiveColumnNotAvailableAgent;
                        }
                        else
                        {
                            panelSCStatus.Visible = false;
                            panelSCStatus.SendToBack();

                            if (direction == WizardDirection.Next)
                                InitSensitiveColumns();
                        }
                        break;
                    }
                case WizardPage.BeforeAfter:
                    {
                        this.previousButton.Enabled = true;
                        this.nextButton.Enabled = true;
                        this.nextButton.Text = "Next";
                        this.cancelButton.Enabled = true;
                        this.Text = _titleRegulation;
                        this.labelTitle.Text = "Before After Data Change";
                        this.labelDescription.Text = "Select the databases you want to audit for Before After Data change, and click configure.";
                        linkMoreInfo.Visible = false;

                        if (!_server.IsDeployed)
                        {
                            panelBADStatus.BringToFront();
                            panelBADStatus.Visible = true;
                            labelBADStatus.Text = CoreConstants.Feature_BeforeAfterNotAvailableVersionUnknown;
                        }
                        else if (!SupportsSensitiveColumns())
                        {
                            panelBADStatus.BringToFront();
                            panelBADStatus.Visible = true;
                            labelBADStatus.Text = CoreConstants.Feature_BeforeAfterNotAvailableAgent;
                        }
                        else
                        {
                            panelBADStatus.Visible = false;
                            panelBADStatus.SendToBack();

                            if (direction == WizardDirection.Next)
                                InitDataChangeColumns();
                        }
                        break;
                    }
                case WizardPage.PermissionsCheck:
                    previousButton.Enabled = true;
                    nextButton.Enabled = false;
                    nextButton.Text = @"Next";
                    cancelButton.Enabled = true;
                    Text = _titlePermissionsCheck;
                    labelTitle.Text = @"Permissions Check";
                    labelDescription.Text = @"Required permissions are checked for proper functioning of SQLcm processes on SQL Server instance to be audited.";
                    linkMoreInfo.Visible = false;
                    permissionsCheck.StartPermissionsCheck(_server);
                    break;
                case WizardPage.Summary:
                    {
                        this.previousButton.Enabled = true;
                        this.nextButton.Enabled = true;
                        this.nextButton.Text = "Finish";
                        this.cancelButton.Enabled = true;
                        this.Text = _titleSummary;
                        this.labelTitle.Text = "Summary";
                        this.labelDescription.Text = "Review the summary of the audit settings you chose for this SQL Server instance and its hosted databases.";
                        linkMoreInfo.Visible = false;
                        LoadSummaryInfo();
                        if (_startPage == StartPage.Database)
                            pictureBox1.Image = Idera.SQLcompliance.Application.GUI.Properties.Resources.Wizard_DB;
                        else
                            pictureBox1.Image = Idera.SQLcompliance.Application.GUI.Properties.Resources.Wizard_Server;
                        break;
                    }
                default:
                    {
                        this.previousButton.Enabled = false;
                        this.nextButton.Enabled = false;
                        this.cancelButton.Enabled = false;
                        linkMoreInfo.Visible = false;
                        break;
                    }
            }
        }

        /// <summary>
        /// Grey Regulation Options for Server and Databases
        /// </summary>
        private void GreyRegulationOptions()
        {
            // Start of Server Node Selection
            if (Globals.isServerNodeSelected)
            {
                ServerRecord tempServer = new ServerRecord();
                RegulationSettings settings;
                bool _privUserServerCheck = false;

                // apply PCI
                if (this._regulationSettings.TryGetValue((int)Regulation.RegulationType.PCI, out settings))
                {
                    tempServer.AuditLogins =
                        ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.Logins)
                         == (int)RegulationSettings.RegulationServerCategory.Logins) || tempServer.AuditLogins;
                    // SQLCM-5375-6.1.4.3-Capture Logout Events at Server level - Updated the RegulationMap Data to include Logouts Events
                    tempServer.AuditLogouts =
                        ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.Logouts)
                         == (int)RegulationSettings.RegulationServerCategory.Logouts) || tempServer.AuditLogouts;
                    tempServer.AuditFailedLogins =
                        ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.FailedLogins)
                         == (int)RegulationSettings.RegulationServerCategory.FailedLogins)
                        || tempServer.AuditFailedLogins;
                    tempServer.AuditDDL =
                        ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory
                              .DatabaseDefinition) == (int)RegulationSettings.RegulationServerCategory
                             .DatabaseDefinition) || tempServer.AuditDDL;
                    tempServer.AuditAdmin =
                        ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.AdminActivity)
                         == (int)RegulationSettings.RegulationServerCategory.AdminActivity) || tempServer.AuditAdmin;
                    tempServer.AuditSecurity =
                        ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.SecurityChanges)
                         == (int)RegulationSettings.RegulationServerCategory.SecurityChanges)
                        || tempServer.AuditSecurity;
                    tempServer.AuditUDE = false || tempServer.AuditSecurity;

                    if ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory
                             .FilterPassedAccess) == (int)RegulationSettings.RegulationServerCategory.FilterPassedAccess
                        || (tempServer.AuditAccessCheck == AccessCheckFilter.SuccessOnly))
                        tempServer.AuditAccessCheck = AccessCheckFilter.SuccessOnly;
                    else if ((settings.ServerCategories
                              & (int)RegulationSettings.RegulationServerCategory.FilterFailedAccess)
                             == (int)RegulationSettings.RegulationServerCategory.FilterFailedAccess
                             || (tempServer.AuditAccessCheck == AccessCheckFilter.FailureOnly))
                        tempServer.AuditAccessCheck = AccessCheckFilter.FailureOnly;
                    else tempServer.AuditAccessCheck = AccessCheckFilter.NoFilter;

                    _privUserServerCheck =
                        ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.PrivelegedUsers)
                         == (int)RegulationSettings.RegulationServerCategory.PrivelegedUsers) || _privUserServerCheck;
                    if (this.MatchServerRegulations(tempServer))
                    {
                        this.checkPCI.Enabled = false;
                        this.checkPCI.Checked = true;
                    }
                    else
                    {
                        this.checkPCI.Enabled = true;
                    }
                }

                // Reset for next settings
                tempServer = new ServerRecord();
                _privUserServerCheck = false;
                // apply HIPAA
                // the OR against the server settings is done because the user can select more than one template.  When more than one template is 
                // selected, the options are combined together
                if (this._regulationSettings.TryGetValue((int)Regulation.RegulationType.HIPAA, out settings))
                {
                    tempServer.AuditLogins =
                        ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.Logins)
                         == (int)RegulationSettings.RegulationServerCategory.Logins) || tempServer.AuditLogins;
                    // SQLCM-5375-6.1.4.3-Capture Logout Events at Server level - Updated the RegulationMap Data to include Logouts Events
                    tempServer.AuditLogouts =
                        ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.Logouts)
                         == (int)RegulationSettings.RegulationServerCategory.Logouts) || tempServer.AuditLogouts;
                    tempServer.AuditFailedLogins =
                        ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.FailedLogins)
                         == (int)RegulationSettings.RegulationServerCategory.FailedLogins)
                        || tempServer.AuditFailedLogins;
                    tempServer.AuditDDL =
                        ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory
                              .DatabaseDefinition) == (int)RegulationSettings.RegulationServerCategory
                             .DatabaseDefinition) || tempServer.AuditDDL;
                    tempServer.AuditAdmin =
                        ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.AdminActivity)
                         == (int)RegulationSettings.RegulationServerCategory.AdminActivity) || tempServer.AuditAdmin;
                    tempServer.AuditSecurity =
                        ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.SecurityChanges)
                         == (int)RegulationSettings.RegulationServerCategory.SecurityChanges)
                        || tempServer.AuditSecurity;
                    tempServer.AuditUDE = false;

                    if (((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory
                              .FilterPassedAccess)
                         == (int)RegulationSettings.RegulationServerCategory.FilterPassedAccess)
                        || (tempServer.AuditAccessCheck == AccessCheckFilter.SuccessOnly))
                        tempServer.AuditAccessCheck = AccessCheckFilter.SuccessOnly;
                    else if (((settings.ServerCategories
                               & (int)RegulationSettings.RegulationServerCategory.FilterFailedAccess)
                              == (int)RegulationSettings.RegulationServerCategory.FilterFailedAccess)
                             || (tempServer.AuditAccessCheck == AccessCheckFilter.FailureOnly))
                        tempServer.AuditAccessCheck = AccessCheckFilter.FailureOnly;
                    else tempServer.AuditAccessCheck = AccessCheckFilter.NoFilter;

                    _privUserServerCheck =
                        ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.PrivelegedUsers)
                         == (int)RegulationSettings.RegulationServerCategory.PrivelegedUsers) || _privUserServerCheck;

                    //checkHIPAA
                    if (this.MatchServerRegulations(tempServer))
                    {
                        this.checkHIPAA.Enabled = false;
                        this.checkHIPAA.Checked = true;
                    }
                    else
                    {
                        this.checkHIPAA.Enabled = true;
                    }
                }

                // Reset for next settings
                tempServer = new ServerRecord();
                _privUserServerCheck = false;
                if (this._regulationSettings.TryGetValue((int)Regulation.RegulationType.DISA, out settings))
                {
                    tempServer.AuditLogins =
                        ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.Logins)
                         == (int)RegulationSettings.RegulationServerCategory.Logins) || tempServer.AuditLogins;
                    // SQLCM-5375-6.1.4.3-Capture Logout Events at Server level - Updated the RegulationMap Data to include Logouts Events
                    tempServer.AuditLogouts =
                        ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.Logouts)
                         == (int)RegulationSettings.RegulationServerCategory.Logouts) || tempServer.AuditLogouts;
                    tempServer.AuditFailedLogins =
                        ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.FailedLogins)
                         == (int)RegulationSettings.RegulationServerCategory.FailedLogins)
                        || tempServer.AuditFailedLogins;
                    tempServer.AuditDDL =
                        ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory
                              .DatabaseDefinition) == (int)RegulationSettings.RegulationServerCategory
                             .DatabaseDefinition) || tempServer.AuditDDL;
                    tempServer.AuditAdmin =
                        ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.AdminActivity)
                         == (int)RegulationSettings.RegulationServerCategory.AdminActivity) || tempServer.AuditAdmin;
                    tempServer.AuditSecurity =
                        ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.SecurityChanges)
                         == (int)RegulationSettings.RegulationServerCategory.SecurityChanges)
                        || tempServer.AuditSecurity;
                    tempServer.AuditUDE =
                        ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.UserDefined)
                         == (int)RegulationSettings.RegulationServerCategory.UserDefined) || tempServer.AuditUDE;

                    if (((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory
                              .FilterPassedAccess)
                         == (int)RegulationSettings.RegulationServerCategory.FilterPassedAccess)
                        || (tempServer.AuditAccessCheck == AccessCheckFilter.SuccessOnly))
                        tempServer.AuditAccessCheck = AccessCheckFilter.SuccessOnly;
                    else if (((settings.ServerCategories
                               & (int)RegulationSettings.RegulationServerCategory.FilterFailedAccess)
                              == (int)RegulationSettings.RegulationServerCategory.FilterFailedAccess)
                             || (tempServer.AuditAccessCheck == AccessCheckFilter.FailureOnly))
                        tempServer.AuditAccessCheck = AccessCheckFilter.FailureOnly;
                    else tempServer.AuditAccessCheck = AccessCheckFilter.NoFilter;

                    _privUserServerCheck =
                        ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.PrivelegedUsers)
                         == (int)RegulationSettings.RegulationServerCategory.PrivelegedUsers) || _privUserServerCheck;

                    if (this.MatchServerRegulations(tempServer))
                    {
                        this.checkDISA.Enabled = false;
                        this.checkDISA.Checked = true;
                    }
                    else
                    {
                        this.checkDISA.Enabled = true;
                    }
                }

                // Reset for next settings
                tempServer = new ServerRecord();
                _privUserServerCheck = false;
                if (this._regulationSettings.TryGetValue((int)Regulation.RegulationType.NERC, out settings))
                {
                    tempServer.AuditLogins =
                        ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.Logins)
                         == (int)RegulationSettings.RegulationServerCategory.Logins) || tempServer.AuditLogins;
                    // SQLCM-5375-6.1.4.3-Capture Logout Events at Server level - Updated the RegulationMap Data to include Logouts Events
                    tempServer.AuditLogouts =
                        ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.Logouts)
                         == (int)RegulationSettings.RegulationServerCategory.Logouts) || tempServer.AuditLogouts;
                    tempServer.AuditFailedLogins =
                        ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.FailedLogins)
                         == (int)RegulationSettings.RegulationServerCategory.FailedLogins)
                        || tempServer.AuditFailedLogins;
                    tempServer.AuditDDL =
                        ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory
                              .DatabaseDefinition) == (int)RegulationSettings.RegulationServerCategory
                             .DatabaseDefinition) || tempServer.AuditDDL;
                    tempServer.AuditAdmin =
                        ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.AdminActivity)
                         == (int)RegulationSettings.RegulationServerCategory.AdminActivity) || tempServer.AuditAdmin;
                    tempServer.AuditSecurity =
                        ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.SecurityChanges)
                         == (int)RegulationSettings.RegulationServerCategory.SecurityChanges)
                        || tempServer.AuditSecurity;
                    tempServer.AuditUDE =
                        ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.UserDefined)
                         == (int)RegulationSettings.RegulationServerCategory.UserDefined) || tempServer.AuditUDE;

                    if (((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory
                              .FilterPassedAccess)
                         == (int)RegulationSettings.RegulationServerCategory.FilterPassedAccess)
                        || (tempServer.AuditAccessCheck == AccessCheckFilter.SuccessOnly))
                        tempServer.AuditAccessCheck = AccessCheckFilter.SuccessOnly;
                    else if (((settings.ServerCategories
                               & (int)RegulationSettings.RegulationServerCategory.FilterFailedAccess)
                              == (int)RegulationSettings.RegulationServerCategory.FilterFailedAccess)
                             || (tempServer.AuditAccessCheck == AccessCheckFilter.FailureOnly))
                        tempServer.AuditAccessCheck = AccessCheckFilter.FailureOnly;
                    else tempServer.AuditAccessCheck = AccessCheckFilter.NoFilter;

                    _privUserServerCheck =
                        ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.PrivelegedUsers)
                         == (int)RegulationSettings.RegulationServerCategory.PrivelegedUsers) || _privUserServerCheck;

                    if (this.MatchServerRegulations(tempServer))
                    {
                        this.checkNERC.Enabled = false;
                        this.checkNERC.Checked = true;
                    }
                    else
                    {
                        this.checkNERC.Enabled = true;
                    }
                }

                // Reset for next settings
                tempServer = new ServerRecord();
                _privUserServerCheck = false;
                if (this._regulationSettings.TryGetValue((int)Regulation.RegulationType.CIS, out settings))
                {
                    tempServer.AuditLogins =
                        ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.Logins)
                         == (int)RegulationSettings.RegulationServerCategory.Logins) || tempServer.AuditLogins;
                    // SQLCM-5375-6.1.4.3-Capture Logout Events at Server level - Updated the RegulationMap Data to include Logouts Events
                    tempServer.AuditLogouts =
                        ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.Logouts)
                         == (int)RegulationSettings.RegulationServerCategory.Logouts) || tempServer.AuditLogouts;
                    tempServer.AuditFailedLogins =
                        ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.FailedLogins)
                         == (int)RegulationSettings.RegulationServerCategory.FailedLogins)
                        || tempServer.AuditFailedLogins;
                    tempServer.AuditDDL =
                        ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory
                              .DatabaseDefinition) == (int)RegulationSettings.RegulationServerCategory
                             .DatabaseDefinition) || tempServer.AuditDDL;
                    tempServer.AuditAdmin =
                        ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.AdminActivity)
                         == (int)RegulationSettings.RegulationServerCategory.AdminActivity) || tempServer.AuditAdmin;
                    tempServer.AuditSecurity =
                        ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.SecurityChanges)
                         == (int)RegulationSettings.RegulationServerCategory.SecurityChanges)
                        || tempServer.AuditSecurity;
                    tempServer.AuditUDE =
                        ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.UserDefined)
                         == (int)RegulationSettings.RegulationServerCategory.UserDefined) || tempServer.AuditUDE;

                    if (((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory
                              .FilterPassedAccess)
                         == (int)RegulationSettings.RegulationServerCategory.FilterPassedAccess)
                        || (tempServer.AuditAccessCheck == AccessCheckFilter.SuccessOnly))
                        tempServer.AuditAccessCheck = AccessCheckFilter.SuccessOnly;
                    else if (((settings.ServerCategories
                               & (int)RegulationSettings.RegulationServerCategory.FilterFailedAccess)
                              == (int)RegulationSettings.RegulationServerCategory.FilterFailedAccess)
                             || (tempServer.AuditAccessCheck == AccessCheckFilter.FailureOnly))
                        tempServer.AuditAccessCheck = AccessCheckFilter.FailureOnly;
                    else tempServer.AuditAccessCheck = AccessCheckFilter.NoFilter;

                    _privUserServerCheck =
                        ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.PrivelegedUsers)
                         == (int)RegulationSettings.RegulationServerCategory.PrivelegedUsers) || _privUserServerCheck;

                    if (this.MatchServerRegulations(tempServer))
                    {
                        this.checkCIS.Enabled = false;
                        this.checkCIS.Checked = true;
                    }
                    else
                    {
                        this.checkCIS.Enabled = true;
                    }
                }

                // Reset for next settings
                tempServer = new ServerRecord();
                _privUserServerCheck = false;
                if (this._regulationSettings.TryGetValue((int)Regulation.RegulationType.SOX, out settings))
                {
                    tempServer.AuditLogins =
                        ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.Logins)
                         == (int)RegulationSettings.RegulationServerCategory.Logins) || tempServer.AuditLogins;
                    // SQLCM-5375-6.1.4.3-Capture Logout Events at Server level - Updated the RegulationMap Data to include Logouts Events
                    tempServer.AuditLogouts =
                        ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.Logouts)
                         == (int)RegulationSettings.RegulationServerCategory.Logouts) || tempServer.AuditLogouts;
                    tempServer.AuditFailedLogins =
                        ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.FailedLogins)
                         == (int)RegulationSettings.RegulationServerCategory.FailedLogins)
                        || tempServer.AuditFailedLogins;
                    tempServer.AuditDDL =
                        ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory
                              .DatabaseDefinition) == (int)RegulationSettings.RegulationServerCategory
                             .DatabaseDefinition) || tempServer.AuditDDL;
                    tempServer.AuditAdmin =
                        ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.AdminActivity)
                         == (int)RegulationSettings.RegulationServerCategory.AdminActivity) || tempServer.AuditAdmin;
                    tempServer.AuditSecurity =
                        ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.SecurityChanges)
                         == (int)RegulationSettings.RegulationServerCategory.SecurityChanges)
                        || tempServer.AuditSecurity;
                    tempServer.AuditUDE =
                        ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.UserDefined)
                         == (int)RegulationSettings.RegulationServerCategory.UserDefined) || tempServer.AuditUDE;

                    if (((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory
                              .FilterPassedAccess)
                         == (int)RegulationSettings.RegulationServerCategory.FilterPassedAccess)
                        || (tempServer.AuditAccessCheck == AccessCheckFilter.SuccessOnly))
                        tempServer.AuditAccessCheck = AccessCheckFilter.SuccessOnly;
                    else if (((settings.ServerCategories
                               & (int)RegulationSettings.RegulationServerCategory.FilterFailedAccess)
                              == (int)RegulationSettings.RegulationServerCategory.FilterFailedAccess)
                             || (tempServer.AuditAccessCheck == AccessCheckFilter.FailureOnly))
                        tempServer.AuditAccessCheck = AccessCheckFilter.FailureOnly;
                    else tempServer.AuditAccessCheck = AccessCheckFilter.NoFilter;

                    _privUserServerCheck =
                        ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.PrivelegedUsers)
                         == (int)RegulationSettings.RegulationServerCategory.PrivelegedUsers) || _privUserServerCheck;
                    if (this.MatchServerRegulations(tempServer))
                    {
                        this.checkSOX.Enabled = false;
                        this.checkSOX.Checked = true;
                    }
                    else
                    {
                        this.checkSOX.Enabled = true;
                    }
                }

                // Reset for next settings
                tempServer = new ServerRecord();
                _privUserServerCheck = false;
                if (this._regulationSettings.TryGetValue((int)Regulation.RegulationType.FERPA, out settings))
                {
                    tempServer.AuditLogins =
                        ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.Logins)
                         == (int)RegulationSettings.RegulationServerCategory.Logins) || tempServer.AuditLogins;
                    tempServer.AuditLogouts =
                        ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.Logouts)
                         == (int)RegulationSettings.RegulationServerCategory.Logouts) || tempServer.AuditLogouts;
                    tempServer.AuditFailedLogins =
                        ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.FailedLogins)
                         == (int)RegulationSettings.RegulationServerCategory.FailedLogins)
                        || tempServer.AuditFailedLogins;
                    tempServer.AuditDDL =
                        ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory
                              .DatabaseDefinition) == (int)RegulationSettings.RegulationServerCategory
                             .DatabaseDefinition) || tempServer.AuditDDL;
                    tempServer.AuditAdmin =
                        ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.AdminActivity)
                         == (int)RegulationSettings.RegulationServerCategory.AdminActivity) || tempServer.AuditAdmin;
                    tempServer.AuditSecurity =
                        ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.SecurityChanges)
                         == (int)RegulationSettings.RegulationServerCategory.SecurityChanges)
                        || tempServer.AuditSecurity;
                    tempServer.AuditUDE =
                        ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.UserDefined)
                         == (int)RegulationSettings.RegulationServerCategory.UserDefined) || tempServer.AuditUDE;

                    if (((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory
                              .FilterPassedAccess)
                         == (int)RegulationSettings.RegulationServerCategory.FilterPassedAccess)
                        || (tempServer.AuditAccessCheck == AccessCheckFilter.SuccessOnly))
                        tempServer.AuditAccessCheck = AccessCheckFilter.SuccessOnly;
                    else if (((settings.ServerCategories
                               & (int)RegulationSettings.RegulationServerCategory.FilterFailedAccess)
                              == (int)RegulationSettings.RegulationServerCategory.FilterFailedAccess)
                             || (tempServer.AuditAccessCheck == AccessCheckFilter.FailureOnly))
                        tempServer.AuditAccessCheck = AccessCheckFilter.FailureOnly;
                    else tempServer.AuditAccessCheck = AccessCheckFilter.NoFilter;

                    _privUserServerCheck =
                        ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.PrivelegedUsers)
                         == (int)RegulationSettings.RegulationServerCategory.PrivelegedUsers) || _privUserServerCheck;

                    if (this.MatchServerRegulations(tempServer))
                    {
                        this.checkFERPA.Enabled = false;
                        this.checkFERPA.Checked = true;
                    }
                    else
                    {
                        this.checkFERPA.Enabled = true;
                    }
                }

                // Reset for next settings
                tempServer = new ServerRecord();
                _privUserServerCheck = false;
                if (this._regulationSettings.TryGetValue((int)Regulation.RegulationType.GDPR, out settings))
                {
                    tempServer.AuditLogins =
                        ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.Logins)
                         == (int)RegulationSettings.RegulationServerCategory.Logins) || tempServer.AuditLogins;
                    tempServer.AuditLogouts =
                        ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.Logouts)
                         == (int)RegulationSettings.RegulationServerCategory.Logouts) || tempServer.AuditLogouts;
                    tempServer.AuditFailedLogins =
                        ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.FailedLogins)
                         == (int)RegulationSettings.RegulationServerCategory.FailedLogins)
                        || tempServer.AuditFailedLogins;
                    tempServer.AuditDDL =
                        ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory
                              .DatabaseDefinition) == (int)RegulationSettings.RegulationServerCategory
                             .DatabaseDefinition) || tempServer.AuditDDL;
                    tempServer.AuditAdmin =
                        ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.AdminActivity)
                         == (int)RegulationSettings.RegulationServerCategory.AdminActivity) || tempServer.AuditAdmin;
                    tempServer.AuditSecurity =
                        ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.SecurityChanges)
                         == (int)RegulationSettings.RegulationServerCategory.SecurityChanges)
                        || tempServer.AuditSecurity;
                    tempServer.AuditUDE =
                        ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.UserDefined)
                         == (int)RegulationSettings.RegulationServerCategory.UserDefined) || tempServer.AuditUDE;

                    if (((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory
                              .FilterPassedAccess)
                         == (int)RegulationSettings.RegulationServerCategory.FilterPassedAccess)
                        || (tempServer.AuditAccessCheck == AccessCheckFilter.SuccessOnly))
                        tempServer.AuditAccessCheck = AccessCheckFilter.SuccessOnly;
                    else if (((settings.ServerCategories
                               & (int)RegulationSettings.RegulationServerCategory.FilterFailedAccess)
                              == (int)RegulationSettings.RegulationServerCategory.FilterFailedAccess)
                             || (tempServer.AuditAccessCheck == AccessCheckFilter.FailureOnly))
                        tempServer.AuditAccessCheck = AccessCheckFilter.FailureOnly;
                    else tempServer.AuditAccessCheck = AccessCheckFilter.NoFilter;

                    _privUserServerCheck =
                        ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.PrivelegedUsers)
                         == (int)RegulationSettings.RegulationServerCategory.PrivelegedUsers) || _privUserServerCheck;
                    // SQlCM 5372 - GDPR 
                    if (_privUserServerCheck)
                    {
                        tempServer.AuditUserAccessCheck = AccessCheckFilter.SuccessOnly;
                    }

                    if (this.MatchServerRegulations(tempServer))
                    {
                        this.checkGDPR.Enabled = false;
                        this.checkGDPR.Checked = true;
                    }
                    else
                    {
                        this.checkGDPR.Enabled = true;
                    }
                }
            }  // End of Server Regulations
            else  // Start of Database Regulations
            {
                var dbRecord = this._databases != null && _databases.Count > 0 ? _databases[0] : null;

                RegulationSettings settings;
                DatabaseRecord tempDb = new DatabaseRecord();
                bool _privUserDbCheck = false;
                // apply PCI
                if (_regulationSettings.TryGetValue((int)Regulation.RegulationType.PCI, out settings))
                {
                    tempDb.AuditDDL =
                        ((settings.DatabaseCategories
                          & (int)RegulationSettings.RegulationDatabaseCategory.DatabaseDefinition)
                         == (int)RegulationSettings.RegulationDatabaseCategory.DatabaseDefinition) || tempDb.AuditDDL;
                    tempDb.AuditSecurity =
                        ((settings.DatabaseCategories
                          & (int)RegulationSettings.RegulationDatabaseCategory.SecurityChanges)
                         == (int)RegulationSettings.RegulationDatabaseCategory.SecurityChanges) || tempDb.AuditSecurity;
                    tempDb.AuditAdmin =
                        ((settings.DatabaseCategories
                          & (int)RegulationSettings.RegulationDatabaseCategory.AdminActivity)
                         == (int)RegulationSettings.RegulationDatabaseCategory.AdminActivity) || tempDb.AuditAdmin;
                    tempDb.AuditDML =
                        ((settings.DatabaseCategories
                          & (int)RegulationSettings.RegulationDatabaseCategory.DatabaseModification)
                         == (int)RegulationSettings.RegulationDatabaseCategory.DatabaseModification) || tempDb.AuditDML;
                    tempDb.AuditSELECT =
                        ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.Select)
                         == (int)RegulationSettings.RegulationDatabaseCategory.Select) || tempDb.AuditSELECT;

                    if (((settings.DatabaseCategories
                          & (int)RegulationSettings.RegulationDatabaseCategory.FilterPassedAccess)
                         == (int)RegulationSettings.RegulationDatabaseCategory.FilterPassedAccess)
                        || (tempDb.AuditAccessCheck == AccessCheckFilter.SuccessOnly))
                        tempDb.AuditAccessCheck = AccessCheckFilter.SuccessOnly;
                    else if (((settings.DatabaseCategories
                               & (int)RegulationSettings.RegulationDatabaseCategory.FilterFailedAccess)
                              == (int)RegulationSettings.RegulationDatabaseCategory.FilterFailedAccess)
                             || (tempDb.AuditAccessCheck == AccessCheckFilter.FailureOnly))
                        tempDb.AuditAccessCheck = AccessCheckFilter.FailureOnly;
                    else tempDb.AuditAccessCheck = AccessCheckFilter.NoFilter;

                    tempDb.AuditCaptureSQL = CoreConstants.AllowCaptureSql
                                             && ((settings.DatabaseCategories
                                                  & (int)RegulationSettings.RegulationDatabaseCategory.SQLText)
                                                 == (int)RegulationSettings.RegulationDatabaseCategory.SQLText)
                                             || tempDb.AuditCaptureSQL;
                    tempDb.AuditCaptureTrans =
                        ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.Transactions)
                         == (int)RegulationSettings.RegulationDatabaseCategory.Transactions)
                        || tempDb.AuditCaptureTrans;
                    tempDb.AuditSensitiveColumns =
                        ((settings.DatabaseCategories
                          & (int)RegulationSettings.RegulationDatabaseCategory.SensitiveColumns)
                         == (int)RegulationSettings.RegulationDatabaseCategory.SensitiveColumns)
                        || tempDb.AuditSensitiveColumns;
                    tempDb.AuditDataChanges =
                        ((settings.DatabaseCategories
                          & (int)RegulationSettings.RegulationDatabaseCategory.BeforeAfterDataChange)
                         == (int)RegulationSettings.RegulationDatabaseCategory.BeforeAfterDataChange)
                        || tempDb.AuditDataChanges;

                    _privUserDbCheck =
                        ((settings.DatabaseCategories
                          & (int)RegulationSettings.RegulationDatabaseCategory.PrivelegedUsers)
                         == (int)RegulationSettings.RegulationDatabaseCategory.PrivelegedUsers) || _privUserDbCheck;

                    // Insert Additional Settings
                    this.InsertAdditionalSettings(tempDb);

                    if (MatchDatabaseRegulations(dbRecord, tempDb))
                    {
                        this.checkPCI.Enabled = false;
                        this.checkPCI.Checked = true;
                    }
                    else
                    {
                        this.checkPCI.Enabled = true;
                    }
                }

                tempDb = new DatabaseRecord();
                _privUserDbCheck = false;
                // apply HIPAA
                // the OR against the server settings is done because the user can select more than one template.  When more than one template is 
                // selected, the options are combined togeher
                if (_regulationSettings.TryGetValue((int)Regulation.RegulationType.HIPAA, out settings))
                {
                    tempDb.AuditDDL =
                        ((settings.DatabaseCategories
                          & (int)RegulationSettings.RegulationDatabaseCategory.DatabaseDefinition)
                         == (int)RegulationSettings.RegulationDatabaseCategory.DatabaseDefinition) || tempDb.AuditDDL;
                    tempDb.AuditSecurity =
                        ((settings.DatabaseCategories
                          & (int)RegulationSettings.RegulationDatabaseCategory.SecurityChanges)
                         == (int)RegulationSettings.RegulationDatabaseCategory.SecurityChanges) || tempDb.AuditSecurity;
                    tempDb.AuditAdmin =
                        ((settings.DatabaseCategories
                          & (int)RegulationSettings.RegulationDatabaseCategory.AdminActivity)
                         == (int)RegulationSettings.RegulationDatabaseCategory.AdminActivity) || tempDb.AuditAdmin;
                    tempDb.AuditDML =
                        ((settings.DatabaseCategories
                          & (int)RegulationSettings.RegulationDatabaseCategory.DatabaseModification)
                         == (int)RegulationSettings.RegulationDatabaseCategory.DatabaseModification) || tempDb.AuditDML;
                    tempDb.AuditSELECT =
                        ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.Select)
                         == (int)RegulationSettings.RegulationDatabaseCategory.Select) || tempDb.AuditSELECT;

                    if (((settings.DatabaseCategories
                          & (int)RegulationSettings.RegulationDatabaseCategory.FilterPassedAccess)
                         == (int)RegulationSettings.RegulationDatabaseCategory.FilterPassedAccess)
                        || (tempDb.AuditAccessCheck == AccessCheckFilter.SuccessOnly))
                        tempDb.AuditAccessCheck = AccessCheckFilter.SuccessOnly;
                    else if (((settings.DatabaseCategories
                               & (int)RegulationSettings.RegulationDatabaseCategory.FilterFailedAccess)
                              == (int)RegulationSettings.RegulationDatabaseCategory.FilterFailedAccess)
                             || (tempDb.AuditAccessCheck == AccessCheckFilter.FailureOnly))
                        tempDb.AuditAccessCheck = AccessCheckFilter.FailureOnly;
                    else tempDb.AuditAccessCheck = AccessCheckFilter.NoFilter;

                    tempDb.AuditCaptureSQL = (CoreConstants.AllowCaptureSql
                                              && ((settings.DatabaseCategories
                                                   & (int)RegulationSettings.RegulationDatabaseCategory.SQLText)
                                                  == (int)RegulationSettings.RegulationDatabaseCategory.SQLText))
                                             || tempDb.AuditCaptureSQL;
                    tempDb.AuditCaptureTrans =
                        ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.Transactions)
                         == (int)RegulationSettings.RegulationDatabaseCategory.Transactions)
                        || tempDb.AuditCaptureTrans;
                    tempDb.AuditSensitiveColumns =
                        ((settings.DatabaseCategories
                          & (int)RegulationSettings.RegulationDatabaseCategory.SensitiveColumns)
                         == (int)RegulationSettings.RegulationDatabaseCategory.SensitiveColumns)
                        || tempDb.AuditSensitiveColumns;
                    tempDb.AuditDataChanges =
                        ((settings.DatabaseCategories
                          & (int)RegulationSettings.RegulationDatabaseCategory.BeforeAfterDataChange)
                         == (int)RegulationSettings.RegulationDatabaseCategory.BeforeAfterDataChange)
                        || tempDb.AuditDataChanges;

                    _privUserDbCheck =
                        ((settings.DatabaseCategories
                          & (int)RegulationSettings.RegulationDatabaseCategory.PrivelegedUsers)
                         == (int)RegulationSettings.RegulationDatabaseCategory.PrivelegedUsers) || _privUserDbCheck;

                    // Insert Additional Settings
                    this.InsertAdditionalSettings(tempDb);

                    if (MatchDatabaseRegulations(dbRecord, tempDb))
                    {
                        this.checkHIPAA.Enabled = false;
                        this.checkHIPAA.Checked = true;
                    }
                    else
                    {
                        this.checkHIPAA.Enabled = true;
                    }
                }

                // Reset Database settings
                tempDb = new DatabaseRecord();
                _privUserDbCheck = false;
                if (_regulationSettings.TryGetValue((int)Regulation.RegulationType.DISA, out settings))
                {
                    tempDb.AuditDDL =
                        ((settings.DatabaseCategories
                          & (int)RegulationSettings.RegulationDatabaseCategory.DatabaseDefinition)
                         == (int)RegulationSettings.RegulationDatabaseCategory.DatabaseDefinition) || tempDb.AuditDDL;
                    tempDb.AuditSecurity =
                        ((settings.DatabaseCategories
                          & (int)RegulationSettings.RegulationDatabaseCategory.SecurityChanges)
                         == (int)RegulationSettings.RegulationDatabaseCategory.SecurityChanges) || tempDb.AuditSecurity;
                    tempDb.AuditAdmin =
                        ((settings.DatabaseCategories
                          & (int)RegulationSettings.RegulationDatabaseCategory.AdminActivity)
                         == (int)RegulationSettings.RegulationDatabaseCategory.AdminActivity) || tempDb.AuditAdmin;
                    tempDb.AuditDML =
                        ((settings.DatabaseCategories
                          & (int)RegulationSettings.RegulationDatabaseCategory.DatabaseModification)
                         == (int)RegulationSettings.RegulationDatabaseCategory.DatabaseModification) || tempDb.AuditDML;
                    tempDb.AuditSELECT =
                        ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.Select)
                         == (int)RegulationSettings.RegulationDatabaseCategory.Select) || tempDb.AuditSELECT;

                    if (((settings.DatabaseCategories
                          & (int)RegulationSettings.RegulationDatabaseCategory.FilterPassedAccess)
                         == (int)RegulationSettings.RegulationDatabaseCategory.FilterPassedAccess)
                        || (tempDb.AuditAccessCheck == AccessCheckFilter.SuccessOnly))
                        tempDb.AuditAccessCheck = AccessCheckFilter.SuccessOnly;
                    else if (((settings.DatabaseCategories
                               & (int)RegulationSettings.RegulationDatabaseCategory.FilterFailedAccess)
                              == (int)RegulationSettings.RegulationDatabaseCategory.FilterFailedAccess)
                             || (tempDb.AuditAccessCheck == AccessCheckFilter.FailureOnly))
                        tempDb.AuditAccessCheck = AccessCheckFilter.FailureOnly;
                    else tempDb.AuditAccessCheck = AccessCheckFilter.NoFilter;

                    tempDb.AuditCaptureSQL = (CoreConstants.AllowCaptureSql
                                              && ((settings.DatabaseCategories
                                                   & (int)RegulationSettings.RegulationDatabaseCategory.SQLText)
                                                  == (int)RegulationSettings.RegulationDatabaseCategory.SQLText))
                                             || tempDb.AuditCaptureSQL;
                    tempDb.AuditCaptureTrans =
                        ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.Transactions)
                         == (int)RegulationSettings.RegulationDatabaseCategory.Transactions)
                        || tempDb.AuditCaptureTrans;
                    tempDb.AuditSensitiveColumns =
                        ((settings.DatabaseCategories
                          & (int)RegulationSettings.RegulationDatabaseCategory.SensitiveColumns)
                         == (int)RegulationSettings.RegulationDatabaseCategory.SensitiveColumns)
                        || tempDb.AuditSensitiveColumns;
                    tempDb.AuditDataChanges =
                        ((settings.DatabaseCategories
                          & (int)RegulationSettings.RegulationDatabaseCategory.BeforeAfterDataChange)
                         == (int)RegulationSettings.RegulationDatabaseCategory.BeforeAfterDataChange)
                        || tempDb.AuditDataChanges;

                    _privUserDbCheck =
                        ((settings.DatabaseCategories
                          & (int)RegulationSettings.RegulationDatabaseCategory.PrivelegedUsers)
                         == (int)RegulationSettings.RegulationDatabaseCategory.PrivelegedUsers) || _privUserDbCheck;

                    // Insert Additional Settings
                    this.InsertAdditionalSettings(tempDb);

                    if (MatchDatabaseRegulations(dbRecord, tempDb))
                    {
                        this.checkDISA.Enabled = false;
                        this.checkDISA.Checked = true;
                    }
                    else
                    {
                        this.checkDISA.Enabled = true;
                    }
                }

                // Reset Database settings
                tempDb = new DatabaseRecord();
                _privUserDbCheck = false;
                if (_regulationSettings.TryGetValue((int)Regulation.RegulationType.NERC, out settings))
                {
                    tempDb.AuditDDL =
                        ((settings.DatabaseCategories
                          & (int)RegulationSettings.RegulationDatabaseCategory.DatabaseDefinition)
                         == (int)RegulationSettings.RegulationDatabaseCategory.DatabaseDefinition) || tempDb.AuditDDL;
                    tempDb.AuditSecurity =
                        ((settings.DatabaseCategories
                          & (int)RegulationSettings.RegulationDatabaseCategory.SecurityChanges)
                         == (int)RegulationSettings.RegulationDatabaseCategory.SecurityChanges) || tempDb.AuditSecurity;
                    tempDb.AuditAdmin =
                        ((settings.DatabaseCategories
                          & (int)RegulationSettings.RegulationDatabaseCategory.AdminActivity)
                         == (int)RegulationSettings.RegulationDatabaseCategory.AdminActivity) || tempDb.AuditAdmin;
                    tempDb.AuditDML =
                        ((settings.DatabaseCategories
                          & (int)RegulationSettings.RegulationDatabaseCategory.DatabaseModification)
                         == (int)RegulationSettings.RegulationDatabaseCategory.DatabaseModification) || tempDb.AuditDML;
                    tempDb.AuditSELECT =
                        ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.Select)
                         == (int)RegulationSettings.RegulationDatabaseCategory.Select) || tempDb.AuditSELECT;

                    if (((settings.DatabaseCategories
                          & (int)RegulationSettings.RegulationDatabaseCategory.FilterPassedAccess)
                         == (int)RegulationSettings.RegulationDatabaseCategory.FilterPassedAccess)
                        || (tempDb.AuditAccessCheck == AccessCheckFilter.SuccessOnly))
                        tempDb.AuditAccessCheck = AccessCheckFilter.SuccessOnly;
                    else if (((settings.DatabaseCategories
                               & (int)RegulationSettings.RegulationDatabaseCategory.FilterFailedAccess)
                              == (int)RegulationSettings.RegulationDatabaseCategory.FilterFailedAccess)
                             || (tempDb.AuditAccessCheck == AccessCheckFilter.FailureOnly))
                        tempDb.AuditAccessCheck = AccessCheckFilter.FailureOnly;
                    else tempDb.AuditAccessCheck = AccessCheckFilter.NoFilter;

                    tempDb.AuditCaptureSQL = (CoreConstants.AllowCaptureSql
                                              && ((settings.DatabaseCategories
                                                   & (int)RegulationSettings.RegulationDatabaseCategory.SQLText)
                                                  == (int)RegulationSettings.RegulationDatabaseCategory.SQLText))
                                             || tempDb.AuditCaptureSQL;
                    tempDb.AuditCaptureTrans =
                        ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.Transactions)
                         == (int)RegulationSettings.RegulationDatabaseCategory.Transactions)
                        || tempDb.AuditCaptureTrans;
                    tempDb.AuditSensitiveColumns =
                        ((settings.DatabaseCategories
                          & (int)RegulationSettings.RegulationDatabaseCategory.SensitiveColumns)
                         == (int)RegulationSettings.RegulationDatabaseCategory.SensitiveColumns)
                        || tempDb.AuditSensitiveColumns;
                    tempDb.AuditDataChanges =
                        ((settings.DatabaseCategories
                          & (int)RegulationSettings.RegulationDatabaseCategory.BeforeAfterDataChange)
                         == (int)RegulationSettings.RegulationDatabaseCategory.BeforeAfterDataChange)
                        || tempDb.AuditDataChanges;

                    _privUserDbCheck =
                        ((settings.DatabaseCategories
                          & (int)RegulationSettings.RegulationDatabaseCategory.PrivelegedUsers)
                         == (int)RegulationSettings.RegulationDatabaseCategory.PrivelegedUsers) || _privUserDbCheck;

                    // Insert Additional Settings
                    this.InsertAdditionalSettings(tempDb);

                    if (MatchDatabaseRegulations(dbRecord, tempDb))
                    {
                        this.checkNERC.Enabled = false;
                        this.checkNERC.Checked = true;
                    }
                    else
                    {
                        this.checkNERC.Enabled = true;
                    }
                }

                // Reset Database settings
                tempDb = new DatabaseRecord();
                _privUserDbCheck = false;

                if (_regulationSettings.TryGetValue((int)Regulation.RegulationType.SOX, out settings))
                {
                    tempDb.AuditDDL =
                        ((settings.DatabaseCategories
                          & (int)RegulationSettings.RegulationDatabaseCategory.DatabaseDefinition)
                         == (int)RegulationSettings.RegulationDatabaseCategory.DatabaseDefinition) || tempDb.AuditDDL;
                    tempDb.AuditSecurity =
                        ((settings.DatabaseCategories
                          & (int)RegulationSettings.RegulationDatabaseCategory.SecurityChanges)
                         == (int)RegulationSettings.RegulationDatabaseCategory.SecurityChanges) || tempDb.AuditSecurity;
                    tempDb.AuditAdmin =
                        ((settings.DatabaseCategories
                          & (int)RegulationSettings.RegulationDatabaseCategory.AdminActivity)
                         == (int)RegulationSettings.RegulationDatabaseCategory.AdminActivity) || tempDb.AuditAdmin;
                    tempDb.AuditDML =
                        ((settings.DatabaseCategories
                          & (int)RegulationSettings.RegulationDatabaseCategory.DatabaseModification)
                         == (int)RegulationSettings.RegulationDatabaseCategory.DatabaseModification) || tempDb.AuditDML;
                    tempDb.AuditSELECT =
                        ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.Select)
                         == (int)RegulationSettings.RegulationDatabaseCategory.Select) || tempDb.AuditSELECT;

                    if (((settings.DatabaseCategories
                          & (int)RegulationSettings.RegulationDatabaseCategory.FilterPassedAccess)
                         == (int)RegulationSettings.RegulationDatabaseCategory.FilterPassedAccess)
                        || (tempDb.AuditAccessCheck == AccessCheckFilter.SuccessOnly))
                        tempDb.AuditAccessCheck = AccessCheckFilter.SuccessOnly;
                    else if (((settings.DatabaseCategories
                               & (int)RegulationSettings.RegulationDatabaseCategory.FilterFailedAccess)
                              == (int)RegulationSettings.RegulationDatabaseCategory.FilterFailedAccess)
                             || (tempDb.AuditAccessCheck == AccessCheckFilter.FailureOnly))
                        tempDb.AuditAccessCheck = AccessCheckFilter.FailureOnly;
                    else tempDb.AuditAccessCheck = AccessCheckFilter.NoFilter;

                    tempDb.AuditCaptureSQL = (CoreConstants.AllowCaptureSql
                                              && ((settings.DatabaseCategories
                                                   & (int)RegulationSettings.RegulationDatabaseCategory.SQLText)
                                                  == (int)RegulationSettings.RegulationDatabaseCategory.SQLText))
                                             || tempDb.AuditCaptureSQL;
                    tempDb.AuditCaptureTrans =
                        ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.Transactions)
                         == (int)RegulationSettings.RegulationDatabaseCategory.Transactions)
                        || tempDb.AuditCaptureTrans;
                    tempDb.AuditSensitiveColumns =
                        ((settings.DatabaseCategories
                          & (int)RegulationSettings.RegulationDatabaseCategory.SensitiveColumns)
                         == (int)RegulationSettings.RegulationDatabaseCategory.SensitiveColumns)
                        || tempDb.AuditSensitiveColumns;
                    tempDb.AuditDataChanges =
                        ((settings.DatabaseCategories
                          & (int)RegulationSettings.RegulationDatabaseCategory.BeforeAfterDataChange)
                         == (int)RegulationSettings.RegulationDatabaseCategory.BeforeAfterDataChange)
                        || tempDb.AuditDataChanges;

                    _privUserDbCheck =
                        ((settings.DatabaseCategories
                          & (int)RegulationSettings.RegulationDatabaseCategory.PrivelegedUsers)
                         == (int)RegulationSettings.RegulationDatabaseCategory.PrivelegedUsers) || _privUserDbCheck;

                    // Insert Additional Settings
                    this.InsertAdditionalSettings(tempDb);

                    if (MatchDatabaseRegulations(dbRecord, tempDb))
                    {
                        this.checkSOX.Enabled = false;
                        this.checkSOX.Checked = true;
                    }
                    else
                    {
                        this.checkSOX.Enabled = true;
                    }
                }

                // Reset Database settings
                tempDb = new DatabaseRecord();
                _privUserDbCheck = false;

                if (_regulationSettings.TryGetValue((int)Regulation.RegulationType.FERPA, out settings))
                {
                    tempDb.AuditDDL =
                        ((settings.DatabaseCategories
                          & (int)RegulationSettings.RegulationDatabaseCategory.DatabaseDefinition)
                         == (int)RegulationSettings.RegulationDatabaseCategory.DatabaseDefinition) || tempDb.AuditDDL;
                    tempDb.AuditSecurity =
                        ((settings.DatabaseCategories
                          & (int)RegulationSettings.RegulationDatabaseCategory.SecurityChanges)
                         == (int)RegulationSettings.RegulationDatabaseCategory.SecurityChanges) || tempDb.AuditSecurity;
                    tempDb.AuditAdmin =
                        ((settings.DatabaseCategories
                          & (int)RegulationSettings.RegulationDatabaseCategory.AdminActivity)
                         == (int)RegulationSettings.RegulationDatabaseCategory.AdminActivity) || tempDb.AuditAdmin;
                    tempDb.AuditDML =
                        ((settings.DatabaseCategories
                          & (int)RegulationSettings.RegulationDatabaseCategory.DatabaseModification)
                         == (int)RegulationSettings.RegulationDatabaseCategory.DatabaseModification) || tempDb.AuditDML;
                    tempDb.AuditSELECT =
                        ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.Select)
                         == (int)RegulationSettings.RegulationDatabaseCategory.Select) || tempDb.AuditSELECT;

                    if (((settings.DatabaseCategories
                          & (int)RegulationSettings.RegulationDatabaseCategory.FilterPassedAccess)
                         == (int)RegulationSettings.RegulationDatabaseCategory.FilterPassedAccess)
                        || (tempDb.AuditAccessCheck == AccessCheckFilter.SuccessOnly))
                        tempDb.AuditAccessCheck = AccessCheckFilter.SuccessOnly;
                    else if (((settings.DatabaseCategories
                               & (int)RegulationSettings.RegulationDatabaseCategory.FilterFailedAccess)
                              == (int)RegulationSettings.RegulationDatabaseCategory.FilterFailedAccess)
                             || (tempDb.AuditAccessCheck == AccessCheckFilter.FailureOnly))
                        tempDb.AuditAccessCheck = AccessCheckFilter.FailureOnly;
                    else tempDb.AuditAccessCheck = AccessCheckFilter.NoFilter;

                    tempDb.AuditCaptureSQL = (CoreConstants.AllowCaptureSql
                                              && ((settings.DatabaseCategories
                                                   & (int)RegulationSettings.RegulationDatabaseCategory.SQLText)
                                                  == (int)RegulationSettings.RegulationDatabaseCategory.SQLText))
                                             || tempDb.AuditCaptureSQL;
                    tempDb.AuditCaptureTrans =
                        ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.Transactions)
                         == (int)RegulationSettings.RegulationDatabaseCategory.Transactions)
                        || tempDb.AuditCaptureTrans;
                    tempDb.AuditSensitiveColumns =
                        ((settings.DatabaseCategories
                          & (int)RegulationSettings.RegulationDatabaseCategory.SensitiveColumns)
                         == (int)RegulationSettings.RegulationDatabaseCategory.SensitiveColumns)
                        || tempDb.AuditSensitiveColumns;
                    tempDb.AuditDataChanges =
                        ((settings.DatabaseCategories
                          & (int)RegulationSettings.RegulationDatabaseCategory.BeforeAfterDataChange)
                         == (int)RegulationSettings.RegulationDatabaseCategory.BeforeAfterDataChange)
                        || tempDb.AuditDataChanges;

                    _privUserDbCheck =
                        ((settings.DatabaseCategories
                          & (int)RegulationSettings.RegulationDatabaseCategory.PrivelegedUsers)
                         == (int)RegulationSettings.RegulationDatabaseCategory.PrivelegedUsers) || _privUserDbCheck;

                    // Insert Additional Settings
                    this.InsertAdditionalSettings(tempDb);

                    if (MatchDatabaseRegulations(dbRecord, tempDb))
                    {
                        this.checkFERPA.Enabled = false;
                        this.checkFERPA.Checked = true;
                    }
                    else
                    {
                        this.checkFERPA.Enabled = true;
                    }
                }

                // Reset Database settings
                tempDb = new DatabaseRecord();
                _privUserDbCheck = false;

                if (_regulationSettings.TryGetValue((int)Regulation.RegulationType.GDPR, out settings))
                {
                    tempDb.AuditDDL =
                        ((settings.DatabaseCategories
                          & (int)RegulationSettings.RegulationDatabaseCategory.DatabaseDefinition)
                         == (int)RegulationSettings.RegulationDatabaseCategory.DatabaseDefinition) || tempDb.AuditDDL;
                    tempDb.AuditSecurity =
                        ((settings.DatabaseCategories
                          & (int)RegulationSettings.RegulationDatabaseCategory.SecurityChanges)
                         == (int)RegulationSettings.RegulationDatabaseCategory.SecurityChanges) || tempDb.AuditSecurity;
                    tempDb.AuditAdmin =
                        ((settings.DatabaseCategories
                          & (int)RegulationSettings.RegulationDatabaseCategory.AdminActivity)
                         == (int)RegulationSettings.RegulationDatabaseCategory.AdminActivity) || tempDb.AuditAdmin;
                    tempDb.AuditDML =
                        ((settings.DatabaseCategories
                          & (int)RegulationSettings.RegulationDatabaseCategory.DatabaseModification)
                         == (int)RegulationSettings.RegulationDatabaseCategory.DatabaseModification) || tempDb.AuditDML;
                    tempDb.AuditSELECT =
                        ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.Select)
                         == (int)RegulationSettings.RegulationDatabaseCategory.Select) || tempDb.AuditSELECT;

                    if (((settings.DatabaseCategories
                          & (int)RegulationSettings.RegulationDatabaseCategory.FilterPassedAccess)
                         == (int)RegulationSettings.RegulationDatabaseCategory.FilterPassedAccess)
                        || (tempDb.AuditAccessCheck == AccessCheckFilter.SuccessOnly))
                        tempDb.AuditAccessCheck = AccessCheckFilter.SuccessOnly;
                    else if (((settings.DatabaseCategories
                               & (int)RegulationSettings.RegulationDatabaseCategory.FilterFailedAccess)
                              == (int)RegulationSettings.RegulationDatabaseCategory.FilterFailedAccess)
                             || (tempDb.AuditAccessCheck == AccessCheckFilter.FailureOnly))
                        tempDb.AuditAccessCheck = AccessCheckFilter.FailureOnly;
                    else tempDb.AuditAccessCheck = AccessCheckFilter.NoFilter;

                    tempDb.AuditCaptureSQL = (CoreConstants.AllowCaptureSql
                                              && ((settings.DatabaseCategories
                                                   & (int)RegulationSettings.RegulationDatabaseCategory.SQLText)
                                                  == (int)RegulationSettings.RegulationDatabaseCategory.SQLText))
                                             || tempDb.AuditCaptureSQL;
                    tempDb.AuditCaptureTrans =
                        ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.Transactions)
                         == (int)RegulationSettings.RegulationDatabaseCategory.Transactions)
                        || tempDb.AuditCaptureTrans;
                    tempDb.AuditSensitiveColumns =
                        ((settings.DatabaseCategories
                          & (int)RegulationSettings.RegulationDatabaseCategory.SensitiveColumns)
                         == (int)RegulationSettings.RegulationDatabaseCategory.SensitiveColumns)
                        || tempDb.AuditSensitiveColumns;
                    tempDb.AuditDataChanges =
                        ((settings.DatabaseCategories
                          & (int)RegulationSettings.RegulationDatabaseCategory.BeforeAfterDataChange)
                         == (int)RegulationSettings.RegulationDatabaseCategory.BeforeAfterDataChange)
                        || tempDb.AuditDataChanges;

                    _privUserDbCheck =
                        ((settings.DatabaseCategories
                          & (int)RegulationSettings.RegulationDatabaseCategory.PrivelegedUsers)
                         == (int)RegulationSettings.RegulationDatabaseCategory.PrivelegedUsers) || _privUserDbCheck;

                    // Insert Additional Settings
                    this.InsertAdditionalSettings(tempDb);

                    if (MatchDatabaseRegulations(dbRecord, tempDb))
                    {
                        this.checkGDPR.Enabled = false;
                        this.checkGDPR.Checked = true;
                    }
                    else
                    {
                        this.checkGDPR.Enabled = true;
                    }
                }
            }  // End of Database Regulations
        }

        /// <summary>
        /// Insert Additional Settings
        /// </summary>
        /// <param name="tempDb"></param>
        private void InsertAdditionalSettings(DatabaseRecord tempDb)
        {
            if (!(this.checkPCI.Checked || this.checkHIPAA.Checked || this.checkDISA.Checked || this.checkNERC.Checked || this.checkSOX.Checked
                  || this.checkFERPA.Checked || this.checkGDPR.Checked || this.checkCustom.Checked))
            {
                tempDb.AuditDDL = true;
                tempDb.AuditSecurity = true;
                tempDb.AuditAdmin = true;
                tempDb.AuditAccessCheck = AccessCheckFilter.SuccessOnly;
            }
        }

        /// <summary>
        /// Match Database Regulations Settings
        /// </summary>
        /// <param name="dbRecord"></param>
        /// <param name="tempDb"></param>
        /// <returns></returns>
        private static bool MatchDatabaseRegulations(DatabaseRecord dbRecord, DatabaseRecord tempDb)
        {
            return dbRecord != null && ((!tempDb.AuditDDL || dbRecord.AuditDDL == tempDb.AuditDDL)
                                        && (!tempDb.AuditSecurity
                                            || dbRecord.AuditSecurity == tempDb.AuditSecurity)
                                        && (!tempDb.AuditAdmin || dbRecord.AuditAdmin == tempDb.AuditAdmin)
                                        && (!tempDb.AuditDML || dbRecord.AuditDML == tempDb.AuditDML)
                                        && (!tempDb.AuditSELECT || dbRecord.AuditSELECT == tempDb.AuditSELECT)
                                        && (tempDb.AuditAccessCheck == AccessCheckFilter.NoFilter
                                            || dbRecord.AuditAccessCheck == tempDb.AuditAccessCheck)
                                        && (!tempDb.AuditCaptureSQL
                                            || dbRecord.AuditCaptureSQL == tempDb.AuditCaptureSQL)
                                        && (!tempDb.AuditCaptureTrans
                                            || dbRecord.AuditCaptureTrans == tempDb.AuditCaptureTrans)
                                        && (!tempDb.AuditSensitiveColumns
                                            || dbRecord.AuditSensitiveColumns == tempDb.AuditSensitiveColumns)
                                        && (!tempDb.AuditDataChanges
                                            || dbRecord.AuditDataChanges == tempDb.AuditDataChanges));
        }

        /// <summary>
        /// Match Server Regulation Settings
        /// </summary>
        /// <param name="tempServer"></param>
        /// <returns></returns>
        private bool MatchServerRegulations(ServerRecord tempServer)
        {
            return (!tempServer.AuditLogins || this._server.AuditLogins == tempServer.AuditLogins)
                   && (!tempServer.AuditLogouts || this._server.AuditLogouts == tempServer.AuditLogouts)
                   && (!tempServer.AuditFailedLogins || this._server.AuditFailedLogins == tempServer.AuditFailedLogins)
                   && (!tempServer.AuditDDL || this._server.AuditDDL == tempServer.AuditDDL)
                   && (!tempServer.AuditAdmin || this._server.AuditAdmin == tempServer.AuditAdmin)
                   && (!tempServer.AuditSecurity || this._server.AuditSecurity == tempServer.AuditSecurity)
                   && (!tempServer.AuditUDE || this._server.AuditUDE == tempServer.AuditUDE)
                   && (tempServer.AuditAccessCheck == AccessCheckFilter.NoFilter || this._server.AuditAccessCheck == tempServer.AuditAccessCheck);
        }

        /// <summary>
        /// Unregister Deselection Manager Events
        /// </summary>
        private void UnregisterDeselectionManagerEvents()
        {
            this._deselectionManager.UnRegisterCheckbox(this.chkServerAuditLogins, DeselectControls.ServerLogins);
            this._deselectionManager.UnRegisterCheckbox(this.chkServerAuditLogouts, DeselectControls.ServerLogouts);
            this._deselectionManager.UnRegisterCheckbox(this.chkServerAuditFailedLogins, DeselectControls.ServerFailedLogins);
            this._deselectionManager.UnRegisterCheckbox(this.chkServerAuditSecurity, DeselectControls.ServerSecurityChanges);
            this._deselectionManager.UnRegisterCheckbox(this.chkServerAuditDDL, DeselectControls.ServerDatabaseDefinition);
            this._deselectionManager.UnRegisterCheckbox(this.chkServerAuditAdmin, DeselectControls.ServerAdministrativeActivities);
            this._deselectionManager.UnRegisterCheckbox(this.chkServerAccessCheckFilter, DeselectControls.ServerFilterEvents);
            this._deselectionManager.UnRegisterRadioButton(this.rbServerAuditSuccessfulOnly, DeselectControls.ServerFilterEventsPassOnly);
            this._deselectionManager.UnRegisterRadioButton(this.rbAuditFailedOnly, DeselectControls.ServerFilterEventsFailedOnly);
            this._deselectionManager.UnRegisterCheckbox(this.chkServerAuditUDE, DeselectControls.ServerUserDefined);

            _deselectionManager.UnRegisterCheckbox(chkDBAccessCheckFilter, DeselectControls.DbFilterEvents);
            _deselectionManager.UnRegisterRadioButton(rbDBAuditFailedOnly, DeselectControls.DbFilterEventsFailedOnly);
            _deselectionManager.UnRegisterRadioButton(rbDBAuditSuccessfulOnly, DeselectControls.DbFilterEventsPassOnly);
            _deselectionManager.UnRegisterCheckbox(chkDBAuditCaptureSQL, DeselectControls.DbCaptureSqlDmlSelect);
            _deselectionManager.UnRegisterCheckbox(chkDBAuditCaptureTrans, DeselectControls.DbCaptureSqlTransactionStatus);
            _deselectionManager.UnRegisterCheckbox(chkDBCaptureDDL, DeselectControls.DbCaptureSqlDdlSecurity);
            _deselectionManager.UnRegisterCheckbox(chkDBAuditDDL, DeselectControls.DbDatabaseDefinition);
            _deselectionManager.UnRegisterCheckbox(chkDBAuditSecurity, DeselectControls.DbSecurityChanges);
            _deselectionManager.UnRegisterCheckbox(chkDBAuditAdmin, DeselectControls.DbAdministrativeActivities);
            _deselectionManager.UnRegisterCheckbox(chkDBAuditDML, DeselectControls.DbDatabaseModifications);
            _deselectionManager.UnRegisterCheckbox(chkDBAuditSELECT, DeselectControls.DbDatabaseSelect);


            _deselectionManager.UnRegisterCheckbox(_chkDBAccessCheckFilter, DeselectControls.DbFilterEvents);
            _deselectionManager.UnRegisterRadioButton(_rbDBAuditFailedOnly, DeselectControls.DbFilterEventsFailedOnly);
            _deselectionManager.UnRegisterRadioButton(_rbDBAuditSuccessfulOnly, DeselectControls.DbFilterEventsPassOnly);
            _deselectionManager.UnRegisterCheckbox(_chkCaptureSQL, DeselectControls.DbCaptureSqlDmlSelect);
            _deselectionManager.UnRegisterCheckbox(_chkCaptureTrans, DeselectControls.DbCaptureSqlTransactionStatus);
            _deselectionManager.UnRegisterCheckbox(_chkDBCaptureDDL, DeselectControls.DbCaptureSqlDdlSecurity);
            _deselectionManager.UnRegisterCheckbox(_chkAuditDDL, DeselectControls.DbDatabaseDefinition);
            _deselectionManager.UnRegisterCheckbox(_chkAuditSecurityChanges, DeselectControls.DbSecurityChanges);
            _deselectionManager.UnRegisterCheckbox(_chkAuditAdminActivity, DeselectControls.DbAdministrativeActivities);
            _deselectionManager.UnRegisterCheckbox(_chkAuditDML, DeselectControls.DbDatabaseModifications);
            _deselectionManager.UnRegisterCheckbox(_chkAuditSelect, DeselectControls.DbDatabaseSelect);


            _deselectionManager.UnRegisterCheckbox(_checkLogins, DeselectControls.ServerLogins);
            _deselectionManager.UnRegisterCheckbox(_checkLogouts, DeselectControls.ServerLogouts);
            _deselectionManager.UnRegisterCheckbox(_checkFailedLogins, DeselectControls.ServerFailedLogins);
            _deselectionManager.UnRegisterCheckbox(_checkSecurityChanges, DeselectControls.ServerSecurityChanges);
            _deselectionManager.UnRegisterCheckbox(_checkDDL, DeselectControls.ServerDatabaseDefinition);
            _deselectionManager.UnRegisterCheckbox(_checkAdministrativeActivities, DeselectControls.ServerAdministrativeActivities);
            _deselectionManager.UnRegisterCheckbox(_chkServerAccessCheckFilter, DeselectControls.ServerFilterEvents);
            _deselectionManager.UnRegisterRadioButton(_rbServerAuditSuccessfulOnly, DeselectControls.ServerFilterEventsPassOnly);
            _deselectionManager.UnRegisterRadioButton(_rbAuditFailedOnly, DeselectControls.ServerFilterEventsFailedOnly);
            _deselectionManager.UnRegisterCheckbox(_checkUserDefinedEvents, DeselectControls.ServerUserDefined);

            _deselectionManager.UnRegisterCheckbox(chkUserAuditLogins, DeselectControls.ServerUserLogins);
            _deselectionManager.UnRegisterCheckbox(chkUserAuditLogouts, DeselectControls.ServerUserLogouts);
            _deselectionManager.UnRegisterCheckbox(chkUserAuditFailedLogins, DeselectControls.ServerUserFailedLogins);
            _deselectionManager.UnRegisterCheckbox(chkUserAuditSecurity, DeselectControls.ServerUserSecurityChanges);
            _deselectionManager.UnRegisterCheckbox(chkUserAuditDDL, DeselectControls.ServerUserDatabaseDefinition);
            _deselectionManager.UnRegisterCheckbox(chkUserAuditAdmin, DeselectControls.ServerUserAdministrativeActivities);
            _deselectionManager.UnRegisterCheckbox(chkUserAuditSELECT, DeselectControls.ServerUserDatabaseSelect);
            _deselectionManager.UnRegisterCheckbox(chkUserAuditDML, DeselectControls.ServerUserDatabaseModifications);
            _deselectionManager.UnRegisterCheckbox(chkUserAuditCaptureSQL, DeselectControls.ServerUserCaptureSqlDmlSelect);
            _deselectionManager.UnRegisterCheckbox(chkUserAuditCaptureTrans, DeselectControls.ServerUserCaptureSqlTransactionStatus);
            _deselectionManager.UnRegisterCheckbox(chkUserAccessCheckFilter, DeselectControls.ServerUserFilterEvents);
            _deselectionManager.UnRegisterRadioButton(rbUserAuditSuccessfulOnly, DeselectControls.ServerUserFilterEventsPassOnly);
            _deselectionManager.UnRegisterRadioButton(rbUserAuditFailedOnly, DeselectControls.ServerUserFilterEventsFailedOnly);
            _deselectionManager.UnRegisterCheckbox(chkUserAuditUDE, DeselectControls.ServerUserUde);
            _deselectionManager.UnRegisterCheckbox(chkUserCaptureDDL, DeselectControls.ServerUserCaptureSqlDdlSecurity);

        }

        private void permissionsCheck_OnPermissionsCheckCompleted(PermissionsCheck.Status checkStatus)
        {
            if (_currentPage == WizardPage.PermissionsCheck)
                nextButton.Enabled = !permissionsCheck.IsUserActivityBlocked;
        }

        private void Form_RegisterWizard_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            switch (_currentPage)
            {
                case WizardPage.ServerName:
                    {
                        HelpAlias.ShowHelp(this, HelpAlias.SSHELP_Form_ConfigWizard_AddServer);
                        break;
                    }
                case WizardPage.ExistingDatabase:
                    {
                        HelpAlias.ShowHelp(this, HelpAlias.SSHELP_Form_ConfigWizard_ExistingDatabase);
                        break;
                    }
                case WizardPage.IncompatibleDatabase:
                    {
                        HelpAlias.ShowHelp(this, HelpAlias.SSHELP_Form_ConfigWizard_IncompatibleDatabase);
                        break;
                    }
                case WizardPage.AgentDeployment:
                    {
                        HelpAlias.ShowHelp(this, HelpAlias.SSHELP_Form_ConfigWizard_Deploy);
                        break;
                    }
                case WizardPage.AgentCredentials:
                    {
                        HelpAlias.ShowHelp(this, HelpAlias.SSHELP_Form_ConfigWizard_Account);
                        break;
                    }
                case WizardPage.AgentTraceDirectory:
                    {
                        HelpAlias.ShowHelp(this, HelpAlias.SSHELP_Form_ConfigWizard_Trace);
                        break;
                    }
                case WizardPage.ServerEvents:
                    {
                        HelpAlias.ShowHelp(this, HelpAlias.SSHELP_Form_ConfigWizard_ServerActivities);
                        break;
                    }
                case WizardPage.PrivilegedUsers:
                    {
                        HelpAlias.ShowHelp(this, HelpAlias.SSHELP_Form_ConfigWizard_PrivUsers);
                        break;
                    }
                case WizardPage.PrivilegedUserEvents:
                    {
                        HelpAlias.ShowHelp(this, HelpAlias.SSHELP_Form_ConfigWizard_PrivUserSettings);
                        break;
                    }
                case WizardPage.RepositoryPermissions:
                    {
                        HelpAlias.ShowHelp(this, HelpAlias.SSHELP_Form_ConfigWizard_Permissions);
                        break;
                    }
                case WizardPage.Summary:
                    {
                        HelpAlias.ShowHelp(this, HelpAlias.SSHELP_Form_ConfigWizard_Summary);
                        break;
                    }
                case WizardPage.Regulation:
                    {
                        HelpAlias.ShowHelp(this, HelpAlias.SSHELP_Form_ConfigWizard_ApplyRegulation);
                        break;
                    }
                case WizardPage.RegulationGuidelineInfo:
                    {
                        HelpAlias.ShowHelp(this, HelpAlias.SSHELP_Form_ConfigWizard_RegulationInfo);
                        break;
                    }
                case WizardPage.SensitiveColumns:
                    {
                        HelpAlias.ShowHelp(this, HelpAlias.SSHELP_Form_ConfigWizard_SensitiveColumns);
                        break;
                    }
                case WizardPage.LicenseLimit:
                    {
                        HelpAlias.ShowHelp(this, HelpAlias.SSHELP_Form_ConfigWizard_Error);
                        break;
                    }
                case WizardPage.Cluster:
                    {
                        HelpAlias.ShowHelp(this, HelpAlias.SSHELP_Form_ConfigWizard_IsCluster);
                        break;
                    }
                case WizardPage.Auditing:
                    {
                        HelpAlias.ShowHelp(this, HelpAlias.SSHELP_Form_ConfigWizard_AuditLevel);
                        break;
                    }
                case WizardPage.SelectDatabases:
                    {
                        HelpAlias.ShowHelp(this, HelpAlias.SSHELP_Form_ConfigWizard_AddDatabase);
                        break;
                    }
                case WizardPage.DatabaseCategories:
                    {
                        HelpAlias.ShowHelp(this, HelpAlias.SSHELP_Form_ConfigWizard_DatabaseActivities);
                        break;
                    }
                case WizardPage.DMLFilters:
                    {
                        HelpAlias.ShowHelp(this, HelpAlias.SSHELP_Form_ConfigWizard_DMLFilters);
                        break;
                    }
                case WizardPage.TrustedUsers:
                    {
                        HelpAlias.ShowHelp(this, HelpAlias.SSHELP_Form_ConfigWizard_TrustedUsers);
                        break;
                    }
                case WizardPage.ServerTrustedUsers:// v5.6 SQLCM-5373
                    {
                        HelpAlias.ShowHelp(this, HelpAlias.SSHELP_Form_ConfigWizard_ServerTrustedUsers);
                        break;
                    }
            }
            hlpevent.Handled = true;
        }

        private void linkMoreInfo_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            switch (_currentPage)
            {
                case WizardPage.DatabaseCategories:
                    HelpAlias.ShowHelp(this, HelpAlias.SSHELP_AuditingBestPractices);
                    break;
                case WizardPage.ServerEvents:
                    HelpAlias.ShowHelp(this, HelpAlias.SSHELP_AuditingBestPractices);
                    break;
            }
        }

        #endregion

        #region Server
        private void textSQLServer_TextChanged(object sender, EventArgs e)
        {
            nextButton.Enabled = !String.IsNullOrEmpty(textSQLServer.Text);
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            Form_SQLServerBrowse dlg = new Form_SQLServerBrowse(false);
            try
            {
                if (DialogResult.OK == dlg.ShowDialog())
                    textSQLServer.Text = dlg.SelectedServer.ToString();
            }
            catch (Exception ex)
            {
                ErrorMessage.Show(UIConstants.Error_DMOLoadServers, ex.Message);
            }
        }

        private string GetStringTillCharacter(string text, string character)
        {

            if (text.Contains(character))
            {
                return text.Substring(0, text.IndexOf(COMMA_CHARACTER));
            }

            return text;
        }

        private string GetStringAfterCharacter(string text, string character)
        {

            if (text.Contains(character))
            {
                return text.Substring(text.IndexOf(character) + 1).Trim();
            }

            return text;
        }

        private string GetServerFromSqlServerConnectionString(string sqlServerConnectionString)
        {
            string server = GetStringTillCharacter(sqlServerConnectionString, COMMA_CHARACTER);
            return server.Trim().ToUpper();
        }

        private int? GetPortFromSqlServerConnectionString(string sqlServerConnectionString)
        {
            int? instancePort = null;
            int parsedValue = 0;
            string port = GetStringAfterCharacter(sqlServerConnectionString, COMMA_CHARACTER).Trim();
            if (int.TryParse(port, out parsedValue))
            {
                instancePort = parsedValue;
            }
            return instancePort;
        }

        private bool ValidateServerName()
        {
            string localhost = Dns.GetHostName().ToUpper();

            _computer = "";
            _instance = "";
            _instancePort = GetPortFromSqlServerConnectionString(textSQLServer.Text);

            string server = GetServerFromSqlServerConnectionString(textSQLServer.Text);
            _instanceName = textSQLServer.Text;
            textSQLServer.Text = server;
            int pos = server.IndexOf(@"\");

            if (pos == -1)
            {
                if (server == UIConstants.UpperLocal || server == ".")
                {
                    _instance = localhost;
                    textSQLServer.Text = _instance;
                }
                else
                {
                    _instance = server;

                    if (server != localhost)
                    {
                        _computer = server;
                    }
                }
            }
            else if (pos == 0)
            {
                return false;
            }
            else // pos > 0; we have xxx/yyy
            {
                _computer = server.Substring(0, pos);
                _instance = server.Substring(pos + 1);

                if (_instance.Length == 0)
                {
                    return false;
                }
                else
                {
                    if (_computer == UIConstants.UpperLocal || _computer == ".")
                    {
                        _computer = localhost;
                        textSQLServer.Text = _computer + @"\" + _instance;
                    }
                }
            }

            // Is this install to the repository server??
            _repositoryComputer = false;

            string repositoryHost = UIUtils.GetInstanceHost(Globals.SQLcomplianceConfig.Server.ToUpper());
            string host = _computer;
            if (host == "" || host == ".") host = localhost;

            if (host == repositoryHost)
            {
                _repositoryComputer = true;
            }

            // if we got here, instance name is at least a valid format          
            return true;
        }

        private bool DoesDatabaseExist(out bool compatibleSchema)
        {
            bool exists;
            compatibleSchema = false;

            string instance = textSQLServer.Text.Trim().ToUpper();
            string dbName = EventDatabase.GetDatabaseName(instance);
            exists = EventDatabase.DatabaseExists(dbName, Globals.Repository.Connection);

            if (exists)
            {
                // check schema for compatability - equal or upgradeable
                compatibleSchema = EventDatabase.IsUpgradeableSchema(dbName, Globals.Repository.Connection);
                textDatabaseName.Text = dbName;
                textExistingDatabase.Text = dbName;
            }
            return exists;
        }
        #endregion

        #region Auditing

        private void radioDefault_CheckedChanged(object sender, EventArgs e)
        {
            if (radioDefault.Checked)
                _auditType = AuditingType.Default;
            else if (radioCustom.Checked)
                _auditType = AuditingType.Custom;
            else
                _auditType = AuditingType.Regulation;
        }

        private void radioCustom_CheckedChanged(object sender, EventArgs e)
        {
            radioDefault_CheckedChanged(null, null);
        }
        #endregion

        #region cluster
        private void checkVirtualServer_CheckedChanged(object sender, EventArgs e)
        {
            _isVirtualServer = checkVirtualServer.Checked;
        }
        #endregion

        #region Agent Credentials

        private bool ValidateAccountName()
        {
            string domain;
            string account;

            string tmp = textServiceAccount.Text.Trim();

            int pos = tmp.IndexOf(@"\");
            if (pos <= 0)
            {
                return false;
            }
            else
            {
                domain = tmp.Substring(0, pos);
                account = tmp.Substring(pos + 1);

                if ((domain == "") || (account == ""))
                    return false;
            }
            return true;
        }
        #endregion

        #region Incompatible Exsiting Repository
        private void radioIncompatible_CheckedChanged(object sender, EventArgs e)
        {
            if (radioIncompatibleCancel.Checked)
                nextButton.Enabled = false;
            else
                nextButton.Enabled = true;
        }
        #endregion

        #region Agent Trace directory
        private void radioSpecifyTrace_CheckChanged(object sender, EventArgs e)
        {
            txtTraceDirectory.Enabled = radioSpecifyTrace.Checked;
        }

        private void ReadSqlVersionName()
        {
            try
            {
                SQLDirect direct = new SQLDirect();
                if (direct.OpenConnection(_instanceName))
                {
                    _sqlVersionName = SQLHelpers.GetSqlVersionName(direct.Connection);
                    direct.CloseConnection();
                }
            }
            catch (Exception ex)
            {
                ErrorMessage.Show(Text, UIConstants.Error_CantReadSqlVersionNameForServer, ex.Message);
            }
        }

        private void SetSqlServerPropertites()
        {
            SQLDirect direct = null;
            try
            {
                direct = new SQLDirect();
                if (direct.OpenConnection(_instanceName))
                {
                    ArrayList values = SQLHelpers.GetServerProperties(direct.Connection, "isClustered", "isHadrEnabled");

                    if (values != null)
                    {
                        _isCluster = Convert.ToBoolean(values[0]);
                        _isHadrEnabled = Convert.ToBoolean(values[1]);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorMessage.Show(Text, UIConstants.Error_CantReadSqlPropertiesForServer, ex.Message);
            }
            finally
            {
                direct.CloseConnection();
            }
        }

        private void AddInstance()
        {
            this.Cursor = Cursors.WaitCursor;
            if (!_licenseExceeded)
            {

                ReadSqlVersionName();
                SetSqlServerPropertites();
                // Save
                if (CreateServer())
                {
                    if (!IsServerConnection)
                    {
                        _server.IsDeployed = false;
                        _server.IsDeployedManually = true;
                        _deploymentSuccessful = false;

                        ServerRecord.SetIsFlags(_server.Instance,
                                                _server.IsDeployed,
                                                _server.IsDeployedManually,
                                                _server.IsRunning,
                                                _server.IsCrippled,
                                                Globals.Repository.Connection);
                    }
                    else
                    {
                        // deploy agent
                        if (_alreadyDeployed)
                        {
                            if (!Activate(textSQLServer.Text))
                            {
                                // Activate first - if cant reach an existing agent then we cant 
                                // do anything - mark as not deployed/running.  Activate requires the
                                //  entry exist in the db to work.
                                _server.IsRunning = false;
                                _server.IsDeployed = false;
                                _deploymentSuccessful = false;
                            }
                            else
                            {
                                _server.IsRunning = true;
                                _server.IsDeployed = true;
                            }
                            ServerRecord.SetIsFlags(_server.Instance,
                                                    _server.IsDeployed,
                                                    _server.IsDeployedManually,
                                                    _server.IsRunning,
                                                    _server.IsCrippled,
                                                    Globals.Repository.Connection);
                        }

                        if ((!_alreadyDeployed) && radioButtonDeployNow.Checked)
                        {
                            _server.IsDeployed = true;

                            ServerRecord.SetIsFlags(_server.Instance,
                                                    _server.IsDeployed,
                                                    _server.IsDeployedManually,
                                                    _server.IsRunning,
                                                    _server.IsCrippled,
                                                    Globals.Repository.Connection);
                            if (DeployAgent())
                            {
                                // agent deployed - update all instances on that computer
                                ICollection servers = ServerRecord.GetServers(Globals.Repository.Connection, true);

                                foreach (ServerRecord srvrec in servers)
                                {
                                    if (srvrec.InstanceServer.ToUpper() == _server.InstanceServer.ToUpper())
                                    {
                                        ServerRecord oldServerState = srvrec.Clone();
                                        if (oldServerState.IsDeployed)
                                        {
                                            srvrec.IsRunning = true;
                                            srvrec.IsDeployed = true;
                                            srvrec.AgentTraceDirectory = _server.AgentTraceDirectory;
                                            srvrec.AgentServiceAccount = _server.AgentServiceAccount;
                                            srvrec.Write(oldServerState);

                                            LogRecord.WriteLog(Globals.Repository.Connection,
                                                              LogType.DeployAgent,
                                                              srvrec.Instance,
                                                              "SQLcompliance Agent deployed");
                                        }
                                    }
                                }
                            }
                            else
                            {
                                _server.IsDeployed = false;
                                _deploymentSuccessful = false;

                                ServerRecord.SetIsFlags(_server.Instance,
                                                        _server.IsDeployed,
                                                        _server.IsDeployedManually,
                                                        _server.IsRunning,
                                                        _server.IsCrippled,
                                                        Globals.Repository.Connection);
                            }
                        }
                        // Force a heartbeat for status
                        PingAgent();
                        //SQLCM-6206 - Increasing wait time to 30 seconds depending upon the speed of the sql server to get updated values from agent.
                        for (int iterator = 0; iterator < 15; iterator++)
                        {
                            System.Threading.Thread.Sleep(2000);//sleep for two seconds to wait for the ping to complete.
                            _server.Read(_server.SrvId);
                            if (_server.SqlVersion != 0)
                                break;
                        }
                        UpdateServerTraceOrXESetting(true, _server);
                    }
                }
                else
                {
                    _server = null;
                }
            }
            this.Cursor = Cursors.Default;
        }

        private void UpdateServerTraceOrXESetting(bool isUpdateRequired, ServerRecord server)
        {
            if ((server.SqlVersion <= 10) ||
                (!String.IsNullOrEmpty(server.AgentVersion) && float.Parse(server.AgentVersion.Substring(0, 3)) < 5.4))
            {
                server.AuditTrace = true;
                server.AuditCaptureSQLXE = false;
                if (isUpdateRequired)
                    server.UpdateXESetting(null);
            }
        }

        private bool CreateServer()
        {
            Cursor = Cursors.WaitCursor;

            bool retval = true;

            // Create events database
            string eventsDatabase;

            try
            {
                CreateServerRecord();

                eventsDatabase = EventDatabase.GetDatabaseName(_server.Instance);
                if (!_existingDatabase)
                {
                    // database doesnt already exist
                    EventDatabase.Create(_server.Instance,
                                          eventsDatabase,
                                          _server.DefaultAccess,
                                          Globals.Repository.Connection);
                }
                else
                {
                    // Existing events database case
                    if (radioDeleteDatabase.Checked || radioIncompatibleOverwrite.Checked)
                    {
                        EventDatabase.InitializeExistingEventDatabase(_server.Instance,
                                                                       eventsDatabase,
                                                                       _server.DefaultAccess,
                                                                       Globals.Repository.Connection);
                        // reset watermarks
                        _server.LowWatermark = -2100000000;
                        _server.HighWatermark = -2100000000;
                    }
                    else
                    {
                        // Upgrade existing database to latest version if needed
                        if (!EventDatabase.IsCompatibleSchema(eventsDatabase, Globals.Repository.Connection))
                            EventDatabase.UpgradeEventDatabase(Globals.Repository.Connection, eventsDatabase);

                        int schemaVersion = EventDatabase.GetDatabaseSchemaVersion(Globals.Repository.Connection, eventsDatabase);

                        if (schemaVersion != CoreConstants.RepositoryEventsDbSchemaVersion)
                        {
                            Form_CreateIndex frm = new Form_CreateIndex(true);
                            if (frm.ShowDialog(this) == DialogResult.OK)
                            {
                                EventDatabase.UpdateIndexes(Globals.Repository.Connection, eventsDatabase);
                            }
                            else
                            {
                                SQLcomplianceConfiguration config = new SQLcomplianceConfiguration();
                                config.Read(Globals.Repository.Connection);

                                if (config.IndexStartTime == DateTime.MinValue)
                                {
                                    Form_IndexSchedule schedule = new Form_IndexSchedule();
                                    schedule.ShowDialog(this);
                                    config.IndexStartTime = schedule.IndexStartTime;
                                    config.IndexDuration = schedule.IndexDuration;
                                    config.Write(Globals.Repository.Connection);
                                }
                                SetReindexFlag(true);
                            }
                        }

                        // set watermarks to first and last record in existing database
                        int lowWatermark;
                        int highWatermark;

                        EventDatabase.GetWatermarks(eventsDatabase,
                                                     out lowWatermark,
                                                     out highWatermark,
                                                     Globals.Repository.Connection);

                        _server.LowWatermark = lowWatermark;

                        if (_server.LowWatermark != -2100000000)
                            _server.LowWatermark--;
                        _server.HighWatermark = highWatermark;
                    }

                    // Update SystemDatabase Table
                    EventDatabase.AddSystemDatabase(_server.Instance, eventsDatabase, Globals.Repository.Connection);
                }
                _server.EventDatabase = eventsDatabase;
            }
            catch (Exception ex)
            {
                if (UIUtils.CloseIfConnectionLost()) return false;

                Cursor = Cursors.Default;
                ErrorMessage.Show(this.Text, UIConstants.Error_CantCreateEventsDatabase, ex.Message);
                return false;
            }

            // write server record
            if (!WriteServerRecord())
            {
                retval = false;
            }
            else
            {
                string snapshot = Snapshot.ServerSnapshot(Globals.Repository.Connection, _server, false);

                ServerUpdate.RegisterChange(_server.SrvId,
                                             LogType.NewServer,
                                             _server.Instance,
                                             snapshot);
                AgentStatusMsg.LogStatus(_server.AgentServer,
                                          _server.Instance,
                                          AgentStatusMsg.MsgType.Registered,
                                          Globals.Repository.Connection);
            }

            Cursor = Cursors.Default;

            return retval;
        }

        private bool Activate(string instance)
        {
            bool activated = false;

            try
            {
                // Make sure the service is started
                string agentServer = instance;

                if (agentServer.IndexOf("\\") != -1)
                    agentServer = agentServer.Substring(0, agentServer.IndexOf("\\"));

                try
                {
                    // This will fail across untrusted domains/workgroups.  However, it is
                    //  not a fatal error, so we silently catch and move along.  The true
                    //  point of failure will be in the following AgentManager.Activate().
                    AgentServiceManager serviceManager = new AgentServiceManager(null, null, agentServer, null, null);
                    serviceManager.Start();
                }
                catch (Exception) { }

                // need to register with agent
                AgentManager manager = GUIRemoteObjectsProvider.AgentManager();
                manager.Activate(instance);
                activated = true;
            }
            catch (Exception ex)
            {
                ErrorMessage.Show(UIConstants.Title_Activate,
                                   UIConstants.Error_CantActivate,
                                   UIUtils.TranslateRemotingException(Globals.SQLcomplianceConfig.Server,
                                                                       UIConstants.CollectionServiceName,
                                                                       ex));
            }
            return activated;
        }

        //--------------------------------------------------------------------
        // DeployAgent
        //--------------------------------------------------------------------
        private bool DeployAgent()
        {
            bool success = false;

            ProgressForm progressForm = new ProgressForm(
                  "Deploying SQLcompliance Agent on " + _server.InstanceServer + "...",
                  _server.InstanceServer,
                  _server.AgentServiceAccount,
                  textServicePassword.Text,
                  _server.AgentTraceDirectory,
                  _server.Instance,
                  Globals.SQLcomplianceConfig.Server,
                  DeploymentType.Install);

            progressForm.ShowDialog();

            if (!progressForm.IsCancelled)
            {
                success = progressForm.IsServiceStarted;
            }
            return success;
        }

        private void PingAgent()
        {
            try
            {
                // ping agent
                AgentCommand agentCmd = GUIRemoteObjectsProvider.AgentCommand(_server.AgentServer, _server.AgentPort);
                agentCmd.Ping();
            }
            catch (Exception)
            {
            }
        }

        //--------------------------------------------------------------------
        // CreateServerRecord
        //--------------------------------------------------------------------
        private void CreateServerRecord()
        {
            _server = new ServerRecord();
            _server.Connection = Globals.Repository.Connection;

            // General
            _server.Instance = textSQLServer.Text;
            _server.InstancePort = _instancePort;
            _server.SqlVersionName = _sqlVersionName;
            _server.isClustered = _isVirtualServer;
            _server.IsCluster = _isCluster;
            _server.IsHadrEnabled = _isHadrEnabled;

            if (_computer == "")
            {
                _server.InstanceServer = Dns.GetHostName().ToUpper();
            }
            else
            {
                _server.InstanceServer = _computer;
            }
            _server.AgentServer = _server.InstanceServer;
            _server.Description = textDescription.Text;
            _server.IsEnabled = true;

            if (radioGrantAll.Checked)
                _server.DefaultAccess = 2;
            else if (radioGrantEventsOnly.Checked)
                _server.DefaultAccess = 1;
            else
                _server.DefaultAccess = 0;

            _server.ConfigVersion = 1;
            _server.LastKnownConfigVersion = 0;
            _server.LastConfigUpdate = DateTime.MinValue;
            _server.IsAuditedServer = true;
            _server.IsOnRepositoryHost = this._repositoryComputer;

            // Agent Settings
            _server.AgentServiceAccount = textServiceAccount.Text;

            if (radioDefaultTrace.Checked)
            {
                _server.AgentTraceDirectory = ""; // install will pick default
            }
            else
            {
                _server.AgentTraceDirectory = txtTraceDirectory.Text;
            }

            if (!_alreadyDeployed)
            {
                _server.IsDeployed = false;
                _server.IsDeployedManually = radioButtonDeployManually.Checked;
            }
            else
            {
                _server.IsDeployed = true;
                _server.IsDeployedManually = _alreadyDeployedManually;
            }

            // Audit Settings		
            _server.AuditLogins = chkServerAuditLogins.Checked;
            _server.AuditLogouts = chkServerAuditLogouts.Checked;
            _server.AuditFailedLogins = chkServerAuditFailedLogins.Checked;
            _server.AuditDDL = chkServerAuditDDL.Checked;
            _server.AuditAdmin = chkServerAuditAdmin.Checked;
            _server.AuditSecurity = chkServerAuditSecurity.Checked;
            _server.AuditUDE = chkServerAuditUDE.Checked;

            if (chkServerAccessCheckFilter.Checked)
            {
                if (rbServerAuditSuccessfulOnly.Checked)
                    _server.AuditAccessCheck = AccessCheckFilter.SuccessOnly;
                else
                    _server.AuditAccessCheck = AccessCheckFilter.FailureOnly;
            }
            else
            {
                _server.AuditAccessCheck = AccessCheckFilter.NoFilter;
            }
            _server.AuditExceptions = false;
            _server.AuditUsersList = GetPrivilegedUserProperty();
            _server.AuditUserAll = rbAuditUserAll.Checked;
            _server.AuditUserLogins = chkUserAuditLogins.Checked;
            _server.AuditUserLogouts = chkUserAuditLogouts.Checked;
            _server.AuditUserFailedLogins = chkUserAuditFailedLogins.Checked;
            _server.AuditUserDDL = chkUserAuditDDL.Checked;
            _server.AuditUserSecurity = chkUserAuditSecurity.Checked;
            _server.AuditUserAdmin = chkUserAuditAdmin.Checked;
            _server.AuditUserDML = chkUserAuditDML.Checked;
            _server.AuditUserSELECT = chkUserAuditSELECT.Checked;
            _server.AuditUserUDE = chkUserAuditUDE.Checked;

            if (chkUserAccessCheckFilter.Checked)
            {
                if (rbUserAuditSuccessfulOnly.Checked)
                    _server.AuditUserAccessCheck = AccessCheckFilter.SuccessOnly;
                else
                    _server.AuditUserAccessCheck = AccessCheckFilter.FailureOnly;
            }
            else
            {
                _server.AuditUserAccessCheck = AccessCheckFilter.NoFilter;
            }
            _server.AuditUserCaptureSQL = chkUserAuditCaptureSQL.Checked;
            _server.AuditUserCaptureTrans = chkUserAuditCaptureTrans.Checked;
            _server.AuditUserCaptureDDL = chkUserCaptureDDL.Checked;
            _server.AuditUserExceptions = false;

            //DML only setting
            if (rbAuditUserSelected.Checked && chkUserAuditDML.Checked)
                chkUserAuditCaptureTrans.Enabled = true;
            else
                chkUserAuditCaptureTrans.Enabled = false;

            //DML or SELECT setting
            if (rbAuditUserSelected.Checked && (chkUserAuditDML.Checked || chkUserAuditSELECT.Checked) && CoreConstants.AllowCaptureSql)
                chkUserAuditCaptureSQL.Enabled = true;
            else
            {
                chkUserAuditCaptureSQL.Checked = false;
                chkUserAuditCaptureSQL.Enabled = false;
            }

            //DDL settings
            if (rbAuditUserSelected.Checked && chkUserAuditDDL.Checked && CoreConstants.AllowCaptureSql)
                chkUserCaptureDDL.Enabled = true;
            else
            {
                chkUserCaptureDDL.Checked = false;
                chkUserCaptureDDL.Enabled = false;
            }

            _server.LowWatermark = -2100000000;
            _server.HighWatermark = -2100000000;

            // copy agent properties from existing audited instances   
            if (_existingAuditedServer != null)
            {
                _server.IsRunning = _existingAuditedServer.IsRunning;
                _server.IsCrippled = _existingAuditedServer.IsCrippled;
                _server.InsertAgentProperties = true;
                _server.AgentServer = _existingAuditedServer.AgentServer;
                _server.AgentPort = _existingAuditedServer.AgentPort;
                _server.AgentServiceAccount = _existingAuditedServer.AgentServiceAccount;
                _server.AgentTraceDirectory = _existingAuditedServer.AgentTraceDirectory;
                _server.AgentCollectionInterval = _existingAuditedServer.AgentCollectionInterval;
                _server.AgentForceCollectionInterval = _existingAuditedServer.AgentForceCollectionInterval;
                _server.AgentHeartbeatInterval = _existingAuditedServer.AgentHeartbeatInterval;
                _server.AgentLogLevel = _existingAuditedServer.AgentLogLevel;
                _server.AgentMaxFolderSize = _existingAuditedServer.AgentMaxFolderSize;
                _server.AgentMaxTraceSize = _existingAuditedServer.AgentMaxTraceSize;
                _server.AgentMaxUnattendedTime = _existingAuditedServer.AgentMaxUnattendedTime;
                _server.AgentTraceOptions = _existingAuditedServer.AgentTraceOptions;
                _server.AgentVersion = _existingAuditedServer.AgentVersion;
                _server.TimeLastHeartbeat = _existingAuditedServer.TimeLastHeartbeat;
                _server.IsCompressedFile = _existingAuditedServer.IsCompressedFile;
            }

            if (_convertingNonAudited)
            {
                _server.SrvId = ServerRecord.GetServerId(Globals.Repository.Connection, _server.Instance);
            }
        }

        private void SetReindexFlag(bool reindex)
        {
            // check for collection service - cant uninstall if it is down or unreachable
            try
            {
                ServerManager srvManager = GUIRemoteObjectsProvider.ServerManager();
                srvManager.SetReindexFlag(reindex);
            }
            catch (Exception)
            {
                // TODO:  Should we alert the user when we can't talk to the collection server?
            }
        }

        private bool WriteServerRecord()
        {
            bool retval = false;
            bool done;
            string s;

            if (_convertingNonAudited)
            {
                done = _server.Write(_existingServer);

                s = UIConstants.Error_ErrorConvertingServer;
            }
            else
            {
                done = _server.Create(null);
                s = UIConstants.Error_ErrorCreatingServer;
            }

            if (!done)
                ErrorMessage.Show(this.Text, s, ServerRecord.GetLastError());
            else
                retval = true;

            return retval;
        }
        #endregion

        #region Select Databases

        private void LoadDatabaseInfo()
        {
            if (_isUserDatabaseLoaded)
                return;

            Cursor = Cursors.WaitCursor;

            LoadDatabases();

            //don't try to load the system databases if we couldn't load the user databases.
            if (!_isUserDatabaseLoaded)
            {
                _currentPage = WizardPage.LoadDatabaseError;
                _addingDBs = false;
            }
            else
            {
                //we loaded the user databases. Load the system dbs.
                if (!_server.IsHadrEnabled)
                    LoadSystemDatabases();

                //see if there are any unselected user dbs.
                if (listDatabases.Items.Count == 0)
                    //we couldn't load the system databases but we have already selected all the user databases.
                    _currentPage = WizardPage.AllDBAdded;
                else
                    _currentPage = WizardPage.SelectDatabases;
            }
            Cursor = Cursors.Default;
        }

        private void LoadDatabases()
        {
            bool loaded = true;

            if (_server != null)
                _sqlServer.CloseConnection();

            if (_isUserDatabaseLoaded)
            {
                listDatabases.Items.Clear();
                _isUserDatabaseLoaded = false;
            }

            ICollection dbList = null;
            ICollection applicationDbList = null; //SCM-2174

            // load database list via agent (if deployed)
            if (_server.IsDeployed && _server.IsRunning)
            {
                string url = EndPointUrlBuilder.GetUrl(typeof(AgentManager), Globals.SQLcomplianceConfig.Server, Globals.SQLcomplianceConfig.ServerPort);
                try
                {
                    AgentManager manager = GUIRemoteObjectsProvider.AgentManager();
                    if (!_server.IsHadrEnabled)
                    {
                        dbList = manager.GetRawUserDatabases(_server.Instance);
                    }
                    else
                    {
                        dbList = manager.GetRawUserDatabasesForAlwaysOn(_server.Instance, null);
                    }
                }
                catch (Exception ex)
                {

                    ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                             String.Format("LoadDatabases: URL: {0} Instance {1}", url, _server.Instance), ex, ErrorLog.Severity.Warning);
                    dbList = null;
                }
            }

            // try straight connection to SQL Server if agent connection failed
            if (dbList == null)
            {
                if (_sqlServer.OpenConnection(_instanceName))
                {
                    if (!_server.IsHadrEnabled)
                    {
                        dbList = RawSQL.GetUserDatabases(_sqlServer.Connection);
                    }
                    else
                    {
                        dbList = RawSQL.GetUserDatabasesForAlwaysOn(_sqlServer.Connection, _server.Instance);
                    }
                }
                // if (SERVERPROPERTY.IsHadrEnabled =1)
                //if (_sqlServer.OpenConnection(_server.Instance))
                //{
                //   dbList = RawSQL.GetUserDatabases(_sqlServer.Connection);
                //}
            }

            if ((dbList != null))
            {
                if ((dbList.Count != 0))
                {
                    // SQLCM-5.4 SCM-2174 START
                    //refresh the app database list 
                    DatabaseRecord dbrec = new DatabaseRecord();
                    dbrec.Connection = Globals.Repository.Connection;
                    applicationDbList = dbrec.GetDatabaseList(_server.SrvId);
                    //get the secondary database list if present
                    List<DatabaseRecord> secondaryDbRecord = GetSecondaryDatabaseRecord(_server.SrvId, Globals.Repository.Connection);

                    IList appDbLists = null;
                    if (secondaryDbRecord.Count != 0)
                    {
                        appDbLists = new ArrayList();
                        foreach (DatabaseRecord db in secondaryDbRecord)
                        {
                            RawDatabaseObject raw = new RawDatabaseObject();
                            raw.name = db.Name;
                            raw.dbid = db.SqlDatabaseId;
                            appDbLists.Add(raw);
                        }
                        ((ArrayList)dbList).AddRange(appDbLists);

                    }
                    foreach (string appDbName in applicationDbList)
                    {
                        Boolean notExisted = true;
                        foreach (RawDatabaseObject db in dbList)
                        {
                            if (appDbName == db.name)
                            {
                                notExisted = false;
                            }
                        }
                        if (notExisted)
                        {
                            dbrec.GetDeleteSQL(_server.Instance, appDbName);
                        }
                    }
                    // SQLCM-5.4 SCM-2174 END
                    foreach (RawDatabaseObject db in dbList)
                    {
                        // only load if this database doesnt already exist in our other DB
                        // potentially slow but we are using selects from two sql servers
                        DatabaseRecord dbrecs = new DatabaseRecord();
                        dbrecs.Connection = Globals.Repository.Connection;

                        if (!dbrecs.Read(_server.Instance, db.name) &&
                           !(_server.IsSqlSecureDb && SQLRepository.IsSQLsecureOwnedDB(db.name)))
                        {
                            listDatabases.Items.Add(db);
                        }
                    }
                }
            }
            else
            {
                loaded = false;
            }
            _isUserDatabaseLoaded = loaded;
        }

        private void LoadSystemDatabases()
        {
            bool loaded = true;
            // Only load first time server is switched
            if (_isSystemDatabaseLoaded)
                return;

            Cursor = Cursors.WaitCursor;
            ICollection dbList = null;

            // load database list via agent (if deployed)
            if (_server.IsDeployed && _server.IsRunning)
            {
                try
                {
                    AgentManager manager = GUIRemoteObjectsProvider.AgentManager();
                    dbList = manager.GetRawSystemDatabases(_server.Instance);
                }
                catch (Exception)
                {
                    dbList = null;
                }
            }

            // try straight connection to SQL Server if agent connection failed
            if (dbList == null)
            {
                if (_sqlServer.OpenConnection(_server.Instance))
                {
                    dbList = RawSQL.GetSystemDatabases(_sqlServer.Connection);
                }
            }

            if ((dbList != null) && (dbList.Count != 0))
            {
                foreach (RawDatabaseObject db in dbList)
                {
                    // only load if this database doesnt already exist in our other DB
                    // potentially slow but we are using selects from two sql servers
                    DatabaseRecord dbrec = new DatabaseRecord();
                    dbrec.Connection = Globals.Repository.Connection;

                    if (!dbrec.Read(_server.Instance, db.name))
                    {
                        listDatabases.Items.Add(db);
                    }
                }
            }
            else
            {
                loaded = false;
            }
            _isSystemDatabaseLoaded = loaded;
        }

        #region Always On Avalibility

        /// <summary>
        /// Populates always on list.
        /// </summary>
        /// <param name="alwaysOnrecords">List of always on record fields will be: 
        /// 1. is server audited (bool)
        /// 2. database name
        /// 3. avalibility group
        /// 4. node
        /// </param>
        private void LoadAlwaysOnAvalibilityList(IEnumerable<Quadruplet<bool, string, string, string>> alwaysOnrecords)
        {
            Cursor = Cursors.WaitCursor;

            lstAlwaysOn.Items.Clear();
            foreach (var alwaysOnrecord in alwaysOnrecords)
            {
                lstAlwaysOn.Items.Add(new ListViewItem(new[]{string.Empty, // is server audited
                                                                                     alwaysOnrecord.Second, // database name
                                                                                     alwaysOnrecord.Third, // avalibility group
                                                                                     alwaysOnrecord.Fourth},
                                                                                       alwaysOnrecord.First ? @"imgPass" : "(none)")); //node
            }

            Cursor = Cursors.Default;
        }

        private void LoadAVGConfigurationDetail()
        {
            if (_server.IsCluster ||
               (!_server.IsCluster && !_server.IsHadrEnabled))
            {
                return;
            }

            List<string> dbNames = new List<string>();
            this.Cursor = Cursors.WaitCursor;

            if (_server != null)
                _sqlServer.CloseConnection();

            foreach (RawDatabaseObject db in listDatabases.CheckedItems)
            {
                dbNames.Add(db.name);
            }

            if (dbNames.Count < 0)
            {
                this.Cursor = Cursors.Default;
                return;
            }

            //If the SQLServer version is equal or above SQL2012 then only AlwaysOn is available.
            if (_sqlServer.OpenConnection(_instanceName))
            {
                int dot = _sqlServer.Connection.ServerVersion.IndexOf(".");
                int SQLver = Convert.ToInt32(_sqlServer.Connection.ServerVersion.Substring(0, dot));
                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                    String.Format("The SQL Server Version is {0} for SQL Server {1}", _sqlServer.Connection.ServerVersion,
                    _server.Instance),
                    ErrorLog.Severity.Informational);
                if (SQLver < 11)
                {
                    this.Cursor = Cursors.Default;
                    return;
                }
            }

            // load AVG details via agent (if deployed)
            if (_server.IsDeployed && _server.IsRunning)
            {
                string url = EndPointUrlBuilder.GetUrl(typeof(AgentManager), Globals.SQLcomplianceConfig.Server, Globals.SQLcomplianceConfig.ServerPort);
                try
                {
                    AgentManager manager = GUIRemoteObjectsProvider.AgentManager();
                    _dbAVGList = manager.GetRawAVGDetails(_instanceName, dbNames);

                    // get all the details of the secondary node
                    _secondaryRoleDetailsList = manager.GetSecondaryRoleAllowConnections(_instanceName);
                }
                catch (Exception ex)
                {
                    ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                             String.Format("LoadAVGConfigurationDetail: URL: {0} Instance {1}", url, _server.Instance), ex, ErrorLog.Severity.Warning);
                    _dbAVGList = null;
                }
            }

            // try straight connection to SQL Server if agent connection failed
            if (_dbAVGList == null)
            {
                _dbAVGList = RawSQL.GetAvailabilityGroupDetails(_sqlServer.Connection, dbNames);
            }

            this.Cursor = Cursors.Default;
        }

        #endregion

        private void btnSelectAll_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < listDatabases.Items.Count; i++)
            {
                listDatabases.SetItemChecked(i, true);
            }
            nextButton.Enabled = true;
        }

        private void btnUnselectAll_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < listDatabases.Items.Count; i++)
            {
                listDatabases.SetItemChecked(i, false);
            }
            nextButton.Enabled = false;
        }

        private void checkAuditDatabases_CheckedChanged(object sender, EventArgs e)
        {
            groupDatabases.Enabled = checkAuditDatabases.Checked;

            if (!checkAuditDatabases.Checked)
            {
                btnUnselectAll_Click(null, null);
                _addingDBs = false;
                nextButton.Enabled = true;
            }
            else
                nextButton.Enabled = !(listDatabases.CheckedItems.Count == 0);
        }

        private void listDatabases_SelectedIndexChanged(object sender, EventArgs e)
        {
            nextButton.Enabled = (listDatabases.CheckedItems.Count > 0);
        }

        #endregion

        #region AuditType
        private void linkTypical_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            HelpAlias.ShowHelp(this, HelpAlias.SSHELP_Form_ConfigWizard_AuditLevel);
        }
        #endregion

        #region ServerEvents
        private void Click_cbAccessCheckFilter(object sender, EventArgs e)
        {
            if (chkServerAccessCheckFilter.Checked)
            {
                rbAuditFailedOnly.Enabled = true;
                rbServerAuditSuccessfulOnly.Enabled = true;
            }
            else
            {
                rbAuditFailedOnly.Enabled = false;
                rbServerAuditSuccessfulOnly.Enabled = false;
            }
        }
        #endregion

        //SQLCM-5581, 5582
        private void InitTrustedUsers()
        {
            // only load them once.
            if (_loadedTrustedUsers)
                return;
            lstTrustedUsers.BeginUpdate();

            UserList userList = new UserList(_server.AuditTrustedUsersList);

            // Add logins
            foreach (Login l in userList.Logins)
            {
                ListViewItem vi = lstTrustedUsers.Items.Add(l.Name);
                vi.Tag = l.Sid;
                vi.ImageIndex = (int)AppIcons.Img16.WindowsUser;
                //SQLCM-5658
                vi.ForeColor = System.Drawing.Color.Gray;
                vi.UseItemStyleForSubItems = true;
            }

            // Add server roles
            foreach (ServerRole r in userList.ServerRoles)
            {
                ListViewItem vi = lstTrustedUsers.Items.Add(r.Name);
                vi.Tag = r.Id;
                vi.ImageIndex = (int)AppIcons.Img16.Role;
                //SQLCM-5658
                vi.ForeColor = System.Drawing.Color.Gray;
                vi.UseItemStyleForSubItems = true;
            }
            lstTrustedUsers.EndUpdate();

            if (lstTrustedUsers.Items.Count > 0)
            {
                lstTrustedUsers.TopItem.Selected = true;
                btnRemoveTrusted.Enabled = false;
            }
            _loadedTrustedUsers = true;
        }

        #region Privileged Users

        private void InitPrivilegedUsers()
        {
            // only load them once.
            if (_loadedPrivUsers)
                return;
            lstPrivilegedUsers.BeginUpdate();
            // SQLCM-5722: (SQLCM 5.6) Update the Db Privilege Users as well    
            lstPrivilegedUsersdb.BeginUpdate();

            UserList userList = new UserList(_server.AuditUsersList);

            // Add logins
            foreach (Login l in userList.Logins)
            {
                ListViewItem vi = lstPrivilegedUsers.Items.Add(l.Name);
                vi.Tag = l.Sid;
                vi.ImageIndex = (int)AppIcons.Img16.WindowsUser;
                // SQLCM-5722: (SQLCM 5.6) Perform Graying only for new database only
                if (!Globals.isServerNodeSelected)
                {
                    //SQLCM-5658
                    vi.ForeColor = System.Drawing.Color.Gray;
                    vi.UseItemStyleForSubItems = true;
                }
                ListViewItem viDb = lstPrivilegedUsersdb.Items.Add(l.Name);
                viDb.Tag = l.Sid;
                viDb.ImageIndex = (int)AppIcons.Img16.WindowsUser;
                //SQLCM-5658
                viDb.ForeColor = System.Drawing.Color.Gray;
                viDb.UseItemStyleForSubItems = true;

            }

            // Add server roles
            foreach (ServerRole r in userList.ServerRoles)
            {
                ListViewItem vi = lstPrivilegedUsers.Items.Add(r.Name);
                vi.Tag = r.Id;
                vi.ImageIndex = (int)AppIcons.Img16.Role;
                // SQLCM-5722: (SQLCM 5.6) Update the Db Privilege Users as well    
                if (!Globals.isServerNodeSelected)
                {
                    //SQLCM-5658
                    vi.ForeColor = System.Drawing.Color.Gray;
                    vi.UseItemStyleForSubItems = true;
                }

                ListViewItem viDb = lstPrivilegedUsersdb.Items.Add(r.Name);
                viDb.Tag = r.Id;
                viDb.ImageIndex = (int)AppIcons.Img16.Role;
                //SQLCM-5658
                viDb.ForeColor = System.Drawing.Color.Gray;
                viDb.UseItemStyleForSubItems = true;
            }
            lstPrivilegedUsers.EndUpdate();
            lstPrivilegedUsersdb.EndUpdate();

            if (lstPrivilegedUsers.Items.Count > 0)
            {
                lstPrivilegedUsers.TopItem.Selected = true;
                btnRemovePriv.Enabled = false;
            }
            if (checkCustom.Checked)
            {
                if (_template.AuditTemplate.PrivUserConfig != null)
                {
                    UserAuditConfig _userConfig = _template.AuditTemplate.PrivUserConfig;
                    if (_userConfig != null && _userConfig.Categories.Length > 0)
                    {
                        UpdateUserRecord(_userConfig, _server);
                    }
                }
            }
            SetPrilivegedUserSettings();

            // SQLCM-5722: (SQLCM 5.6) Update the Db Privilege Users as well
            if (_databases != null && _databases.Count > 0)
            {
                lstPrivilegedUsersdb.BeginUpdate();

                userList = new UserList(_databases[0].AuditPrivUsersList);

                // Add logins
                foreach (Login l in userList.Logins)
                {
                    var isFound = false;
                    foreach (ListViewItem item in lstPrivilegedUsersdb.Items)
                    {
                        if (item != null && l.Name.Equals(item.Text))
                        {
                            isFound = true;
                            break;
                        }
                    }
                    if (isFound)
                    {
                        continue;
                    }
                    ListViewItem vi = lstPrivilegedUsersdb.Items.Add(l.Name);
                    vi.Tag = l.Sid;
                    vi.ImageIndex = (int)AppIcons.Img16.WindowsUser;
                }

                // Add server roles
                foreach (ServerRole r in userList.ServerRoles)
                {
                    var isFound = false;
                    foreach (ListViewItem item in lstPrivilegedUsersdb.Items)
                    {
                        if (item != null && r.Name.Equals(item.Text))
                        {
                            isFound = true;
                            break;
                        }
                    }
                    if (isFound)
                    {
                        continue;
                    }

                    ListViewItem vi = lstPrivilegedUsersdb.Items.Add(r.Name);
                    vi.Tag = r.Id;
                    vi.ImageIndex = (int)AppIcons.Img16.Role;
                }
                lstPrivilegedUsersdb.EndUpdate();
            }
            if (lstPrivilegedUsersdb.Items.Count > 0)
            {
                if (lstPrivilegedUsersdb.Items.Count > 0)
                {
                    foreach (ListViewItem itm in lstPrivilegedUsersdb.Items)
                    {
                        if (itm.ForeColor != System.Drawing.Color.Gray)
                        {
                            itm.Selected = true;
                            btnRemovePrivdb.Enabled = true;
                            break;
                        }
                    }
                }
            }
            _loadedPrivUsers = true;
        }


        private void LoadPrivilegedUsers(DatabaseRecord db)
        {
            if (_loadedPrivUsers)
                return;
            lstPrivilegedUsers.BeginUpdate();
            // SQLCM-5722: Apply Regulatory Guideline: Privileged users added at server level are not shown at db level while applying a regulatory guideline
            btnRemovePrivdb.Enabled = false;

            UserList userList = new UserList(db.AuditPrivUsersList);
            // Server User List
            var serverUserList = new UserList(_server.AuditUsersList);

            // Add logins
            foreach (Login login in userList.Logins)
            {
                ListViewItem viewItem = lstPrivilegedUsers.Items.Add(login.Name);
                viewItem.Tag = login.Sid;
                viewItem.ImageIndex = (int)AppIcons.Img16.WindowsUser;
                foreach (var serverLogin in serverUserList.Logins)
                {
                    if (login.Name.Equals(serverLogin.Name))
                    {
                        viewItem.ForeColor = System.Drawing.Color.Gray;
                        viewItem.UseItemStyleForSubItems = true;
                        break;
                    }
                }
            }

            // Add server roles
            foreach (ServerRole serverRole in userList.ServerRoles)
            {
                ListViewItem vi = lstPrivilegedUsers.Items.Add(serverRole.FullName);
                vi.Tag = serverRole.Id;
                vi.ImageIndex = (int)AppIcons.Img16.Role;
                foreach (var serverServerRole in serverUserList.ServerRoles)
                {
                    // SQLCM-5868: Roles added to default server settings gets added twice at database level
                    if (serverServerRole.CompareName(serverRole))
                    {
                        vi.ForeColor = System.Drawing.Color.Gray;
                        vi.UseItemStyleForSubItems = true;
                        break;
                    }
                }
            }

            lstPrivilegedUsers.EndUpdate();

            // Select topmost Privilege users database which are not grayed out
            if (lstPrivilegedUsers.Items.Count > 0)
            {
                foreach (ListViewItem itm in lstPrivilegedUsers.Items)
                {
                    if (itm.ForeColor != System.Drawing.Color.Gray)
                    {
                        itm.Selected = true;
                        btnRemovePrivdb.Enabled = true;
                        break;
                    }
                }
            }

            if (checkCustom.Checked)
            {
                if (_template.AuditTemplate.DbLevelConfigs != null)
                {
                    DBAuditConfig[] config = _template.AuditTemplate.DbLevelConfigs;
                    foreach (DBAuditConfig item in config)
                    {
                        if (item.PrivUserConfig != null)
                        {
                            UserAuditConfig _userConfig = item.PrivUserConfig;
                            if (_userConfig != null && _userConfig.Categories.Length > 0)
                            {
                                UpdateDBUserRecord(_userConfig, db);
                            }
                        }
                    }
                }
            }
            SetDatabasePrilivegedUserSettings(db);

            _loadedPrivUsers = true;
        }

        private void SetDatabasePrilivegedUserSettings(DatabaseRecord db)
        {
            if (db.AuditUserAll)
                rbAuditUserAll.Checked = true;
            else if (!_templateDBInitializationCompleted) // Ensure Initialization happens only once
            {
                // Additive Check the UI Controls
                DeselectionManager.SetAdditiveCheckbox(db.AuditUserAdmin, chkUserAuditAdmin);
                DeselectionManager.SetAdditiveCheckbox(db.AuditUserCaptureSQL, chkUserAuditCaptureSQL);
                DeselectionManager.SetAdditiveCheckbox(db.AuditUserCaptureDDL, chkUserCaptureDDL);
                DeselectionManager.SetAdditiveCheckbox(db.AuditUserCaptureTrans, chkUserAuditCaptureTrans);
                DeselectionManager.SetAdditiveCheckbox(db.AuditUserDDL, chkUserAuditDDL);
                DeselectionManager.SetAdditiveCheckbox(db.AuditUserDML, chkUserAuditDML);
                DeselectionManager.SetAdditiveCheckbox(db.AuditUserFailedLogins, chkUserAuditFailedLogins);
                DeselectionManager.SetAdditiveCheckbox(db.AuditUserLogins, chkUserAuditLogins);
                // SQLCM 5563: Disable Logout with Logins for Server settings not properly configured while applying Regulation Guideline at Server Level
                if (db.AuditUserLogins)
                {
                    DeselectionManager.SetAdditiveCheckbox(db.AuditUserLogouts, chkUserAuditLogouts);
                }
                DeselectionManager.SetAdditiveCheckbox(db.AuditUserSecurity, chkUserAuditSecurity);
                DeselectionManager.SetAdditiveCheckbox(db.AuditUserSELECT, chkUserAuditSELECT);
                DeselectionManager.SetAdditiveCheckbox(db.AuditUserUDE, chkUserAuditUDE);

                if (_server.AuditAccessCheck == AccessCheckFilter.NoFilter && _server.AuditUserAccessCheck == AccessCheckFilter.NoFilter && chkUserAccessCheckFilter.Enabled)
                {
                    if (db.AuditUserAccessCheck == AccessCheckFilter.FailureOnly)
                    {
                        rbUserAuditFailedOnly.Checked = true;
                        chkUserAccessCheckFilter.Checked = true;
                        rbUserAuditFailedOnly.Enabled = true;
                        rbUserAuditSuccessfulOnly.Enabled = true;
                    }
                    else if (db.AuditUserAccessCheck == AccessCheckFilter.SuccessOnly)
                    {
                        rbUserAuditSuccessfulOnly.Checked = true;
                        chkUserAccessCheckFilter.Checked = true;
                        rbUserAuditFailedOnly.Enabled = true;
                        rbUserAuditSuccessfulOnly.Enabled = true;
                    }
                    else
                    {
                        chkUserAccessCheckFilter.Checked = false;
                        rbUserAuditFailedOnly.Enabled = false;
                        rbUserAuditSuccessfulOnly.Enabled = false;
                    }
                }
            }
        }

        private void SetPrilivegedUserSettings()
        {
            if (_server.AuditUserAll)
                rbAuditUserAll.Checked = true;
            else if (!_templateSvrInitializationCompleted)
            {
                DeselectionManager.SetAndGreyOutAdditiveCheckbox(_server.AuditUserAdmin, chkUserAuditAdmin);
                DeselectionManager.SetAndGreyOutAdditiveCheckbox(
                    _server.AuditUserCaptureSQL,
                    chkUserAuditCaptureSQL);
                DeselectionManager.SetAndGreyOutAdditiveCheckbox(_server.AuditUserCaptureDDL, chkUserCaptureDDL);
                DeselectionManager.SetAndGreyOutAdditiveCheckbox(
                    _server.AuditUserCaptureTrans,
                    chkUserAuditCaptureTrans);
                DeselectionManager.SetAndGreyOutAdditiveCheckbox(_server.AuditUserDDL, chkUserAuditDDL);
                DeselectionManager.SetAndGreyOutAdditiveCheckbox(_server.AuditUserDML, chkUserAuditDML);
                DeselectionManager.SetAndGreyOutAdditiveCheckbox(
                    _server.AuditUserFailedLogins,
                    chkUserAuditFailedLogins);
                DeselectionManager.SetAndGreyOutAdditiveCheckbox(_server.AuditUserLogins, chkUserAuditLogins);
                DeselectionManager.SetAndGreyOutAdditiveCheckbox(_server.AuditUserLogouts, chkUserAuditLogouts);
                DeselectionManager.SetAndGreyOutAdditiveCheckbox(_server.AuditUserSecurity, chkUserAuditSecurity);
                DeselectionManager.SetAndGreyOutAdditiveCheckbox(_server.AuditUserSELECT, chkUserAuditSELECT);
                DeselectionManager.SetAndGreyOutAdditiveCheckbox(_server.AuditUserUDE, chkUserAuditUDE);
                if (_server.AuditAccessCheck == AccessCheckFilter.NoFilter
                    && _server.AuditUserAccessCheck == AccessCheckFilter.NoFilter && chkUserAccessCheckFilter.Enabled)
                {
                    if (_server.AuditUserAccessCheck == AccessCheckFilter.FailureOnly)
                    {
                        rbUserAuditFailedOnly.Checked = true;
                        chkUserAccessCheckFilter.Checked = true;

                        chkUserAccessCheckFilter.Enabled = false;
                        rbUserAuditFailedOnly.Enabled = false;
                        rbUserAuditSuccessfulOnly.Enabled = false;
                    }
                    else if (_server.AuditUserAccessCheck == AccessCheckFilter.SuccessOnly)
                    {
                        rbUserAuditSuccessfulOnly.Checked = true;
                        chkUserAccessCheckFilter.Checked = true;

                        chkUserAccessCheckFilter.Enabled = false;
                        rbUserAuditFailedOnly.Enabled = false;
                        rbUserAuditSuccessfulOnly.Enabled = false;
                    }
                    else
                    {
                        chkUserAccessCheckFilter.Checked = false;
                        rbUserAuditFailedOnly.Enabled = false;
                        rbUserAuditSuccessfulOnly.Enabled = false;
                    }
                }
                else
                {
                    switch (_server.AuditAccessCheck)
                    {
                        case AccessCheckFilter.NoFilter:
                            switch (_server.AuditUserAccessCheck)
                            {
                                case AccessCheckFilter.SuccessOnly:
                                    rbUserAuditSuccessfulOnly.Checked = true;
                                    chkUserAccessCheckFilter.Checked = true;

                                    chkUserAccessCheckFilter.Enabled = false;
                                    rbUserAuditFailedOnly.Enabled = false;
                                    rbUserAuditSuccessfulOnly.Enabled = false;
                                    break;
                                case AccessCheckFilter.FailureOnly:
                                    rbUserAuditFailedOnly.Checked = true;
                                    chkUserAccessCheckFilter.Checked = true;

                                    chkUserAccessCheckFilter.Enabled = false;
                                    rbUserAuditFailedOnly.Enabled = false;
                                    rbUserAuditSuccessfulOnly.Enabled = false;
                                    break;
                            }

                            break;
                        case AccessCheckFilter.SuccessOnly:
                            rbUserAuditSuccessfulOnly.Checked = true;
                            chkUserAccessCheckFilter.Checked = true;

                            chkUserAccessCheckFilter.Enabled = false;
                            rbUserAuditFailedOnly.Enabled = false;
                            rbUserAuditSuccessfulOnly.Enabled = false;
                            break;
                        case AccessCheckFilter.FailureOnly:
                            rbUserAuditFailedOnly.Checked = true;
                            chkUserAccessCheckFilter.Checked = true;

                            chkUserAccessCheckFilter.Enabled = false;
                            rbUserAuditFailedOnly.Enabled = false;
                            rbUserAuditSuccessfulOnly.Enabled = false;
                            break;
                    }
                }
            }
        }

        private UserList GetPrivilegedDatabaseLevelUserProperty()
        {
            int count = 0;
            UserList ul = new UserList();

            foreach (ListViewItem vi in lstPrivilegedUsersdb.Items)
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
            return (count == 0) ? null : ul;
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
            Form_PrivUser frm = new Form_PrivUser(_server.Instance, true);

            frm.UseAgentEnumeration = _alreadyDeployed;

            //frm.MainForm = this.mainForm;                                                      
            if (DialogResult.OK == frm.ShowDialog())
            {
                lstPrivilegedUsers.BeginUpdate();
                lstPrivilegedUsersdb.BeginUpdate();

                lstPrivilegedUsers.SelectedItems.Clear();
                lstPrivilegedUsersdb.SelectedItems.Clear();
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

                    // SQLCM-5722: (SQLCM 5.6) Update the Db Privilege Users as well for graying if exists or add new entry
                    found = false;
                    foreach (ListViewItem s in lstPrivilegedUsersdb.Items)
                    {
                        if (itm.Text == s.Text)
                        {
                            found = true;
                            s.ForeColor = System.Drawing.Color.Gray;
                            s.UseItemStyleForSubItems = true;
                            break;
                        }
                    }

                    if (!found)
                    {
                        ListViewItem newItem = new ListViewItem(itm.Text);
                        newItem.Tag = itm.Tag;
                        newItem.ImageIndex = itm.ImageIndex;

                        //SQLCM-5658
                        newItem.ForeColor = System.Drawing.Color.Gray;
                        newItem.UseItemStyleForSubItems = true;

                        lstPrivilegedUsersdb.Items.Add(newItem);
                    }
                }

                lstPrivilegedUsers.EndUpdate();
                lstPrivilegedUsersdb.EndUpdate();

                if (lstPrivilegedUsers.Items.Count > 0)
                {
                    lstPrivilegedUsers.TopItem.Selected = true;
                    btnRemovePriv.Enabled = true;
                }
                // Select topmost Privilege users database
                if (lstPrivilegedUsersdb.Items.Count > 0)
                {
                    foreach (ListViewItem itm in lstPrivilegedUsersdb.Items)
                    {
                        if (itm.ForeColor != System.Drawing.Color.Gray)
                        {
                            itm.Selected = true;
                            btnRemovePrivdb.Enabled = true;
                            break;
                        }
                    }
                }

            }
        }
        private void btnAddPrivdb_Click(object sender, EventArgs e)
        {
            Form_PrivUser frm = new Form_PrivUser(_server.Instance, true);

            frm.UseAgentEnumeration = _alreadyDeployed;

            //frm.MainForm = this.mainForm;                                                      
            if (DialogResult.OK == frm.ShowDialog())
            {
                lstPrivilegedUsersdb.BeginUpdate();

                lstPrivilegedUsersdb.SelectedItems.Clear();

                foreach (ListViewItem itm in frm.listSelected.Items)
                {
                    bool found = false;
                    foreach (ListViewItem s in lstPrivilegedUsersdb.Items)
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
                        lstPrivilegedUsersdb.Items.Add(newItem);
                    }
                }

                lstPrivilegedUsersdb.EndUpdate();

                if (lstPrivilegedUsersdb.Items.Count > 0)
                {
                    if (lstPrivilegedUsersdb.Items.Count > 0)
                    {
                        foreach (ListViewItem itm in lstPrivilegedUsersdb.Items)
                        {
                            if (itm.ForeColor != System.Drawing.Color.Gray)
                            {
                                itm.Selected = true;
                                btnRemovePrivdb.Enabled = true;
                                break;
                            }
                        }
                    }
                }
            }
        }
        private void btnRemovePrivdb_Click(object sender, EventArgs e)
        {
            lstPrivilegedUsersdb.BeginUpdate();

            if (lstPrivilegedUsersdb.SelectedItems.Count == 0)
                return;

            int ndx = lstPrivilegedUsersdb.SelectedIndices[0];

            foreach (ListViewItem priv in lstPrivilegedUsersdb.SelectedItems)
            {
                priv.Remove();
            }

            lstPrivilegedUsersdb.EndUpdate();

            // reset selected item
            if (lstPrivilegedUsersdb.Items.Count != 0)
            {
                lstPrivilegedUsersdb.Focus();
                if (ndx >= lstPrivilegedUsersdb.Items.Count)
                {
                    lstPrivilegedUsersdb.Items[lstPrivilegedUsersdb.Items.Count - 1].Selected = true;
                }
                else
                    lstPrivilegedUsersdb.Items[ndx].Selected = true;
            }
            else
            {
                btnRemovePrivdb.Enabled = false;
            }
        }

        private void btnRemovePriv_Click(object sender, EventArgs e)
        {
            // Add Graying Logic in Privilege USers
            var deselectOptions = DeselectOptions.None;
            if (Globals.isServerNodeSelected)
            {
                // Popup message to request user for deselection at the database level
                var selectionLogic = new DeselectionLogic(
                    "Privileged Users",
                    "You are removing Privileged Users. The action should:",
                    "Privileged Users",
                    "Deselect Server level only",
                    DeselectOptions.CurrentLevelOnly,
                    "Deselect Server and All Databases",
                    DeselectOptions.OtherLevels);

                var frmSelectionLogicDialog = new Form_SelectionLogicDialog(selectionLogic);
                var isDeselected = DialogResult.OK == frmSelectionLogicDialog.ShowDialog();
                if (!isDeselected)
                {
                    return;
                }
                deselectOptions = frmSelectionLogicDialog.SelectedDeselectOptions;
                if (lstPrivilegedUsers.SelectedItems.Count == 0)
                {
                    btnRemovePriv.Enabled = false;
                    return;
                }
            }

            lstPrivilegedUsers.BeginUpdate();

            if (lstPrivilegedUsers.SelectedItems.Count == 0)
                return;

            int ndx = lstPrivilegedUsers.SelectedIndices[0];

            foreach (ListViewItem priv in lstPrivilegedUsers.SelectedItems)
            {
                priv.Remove();
                if (deselectOptions == DeselectOptions.None)
                {
                    continue;
                }
                if (deselectOptions != DeselectOptions.OtherLevels)
                {
                    lstPrivilegedUsersdb.BeginUpdate();
                    foreach (ListViewItem item in lstPrivilegedUsersdb.Items)
                    {
                        if (item.Text == priv.Text)
                        {
                            item.ForeColor = DefaultForeColor;
                            break;
                        }
                    }
                    lstPrivilegedUsersdb.EndUpdate();
                    continue;
                }
                lstPrivilegedUsersdb.BeginUpdate();
                foreach (ListViewItem item in lstPrivilegedUsersdb.Items)
                {
                    if (item.Text == priv.Text)
                    {
                        item.Remove();
                        break;
                    }
                }
                lstPrivilegedUsersdb.EndUpdate();
            }

            lstPrivilegedUsers.EndUpdate();

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
                btnRemovePriv.Enabled = false;
            }
        }
        #endregion

        #region Privileged User Events
        private void rbAuditUserSelected_CheckedChanged(object sender, EventArgs e)
        {
            grpAuditUserActivity.Enabled = rbAuditUserSelected.Checked;
        }

        private void chkAuditUserDML_CheckedChanged(object sender, EventArgs e)
        {
            //DML only setting
            if (rbAuditUserSelected.Checked && chkUserAuditDML.Checked)
                chkUserAuditCaptureTrans.Enabled = true;
            else
            {
                chkUserAuditCaptureTrans.Checked = false;
                chkUserAuditCaptureTrans.Enabled = false;
            }

            //DML or SELECT Setting
            if (rbAuditUserSelected.Checked && (chkUserAuditDML.Checked || chkUserAuditSELECT.Checked) && CoreConstants.AllowCaptureSql)
                chkUserAuditCaptureSQL.Enabled = true;
            else
            {
                chkUserAuditCaptureSQL.Checked = false;
                chkUserAuditCaptureSQL.Enabled = false;
            }

            //DDL Setting
            if (rbAuditUserSelected.Checked && chkUserAuditDDL.Checked && CoreConstants.AllowCaptureSql)
                chkUserCaptureDDL.Enabled = true;
            else
            {
                chkUserCaptureDDL.Checked = false;
                chkUserCaptureDDL.Enabled = false;
            }
        }

        private void Click_chkUserAccessCheckFilter(object sender, EventArgs e)
        {
            UpdateUserAccessCheckFilterDependencies();
        }

        private void UpdateUserAccessCheckFilterDependencies()
        {
            if (chkUserAccessCheckFilter.Checked)
            {
                rbUserAuditFailedOnly.Enabled = true;
                rbUserAuditSuccessfulOnly.Enabled = true;
            }
            else
            {
                rbUserAuditFailedOnly.Enabled = false;
                rbUserAuditSuccessfulOnly.Enabled = false;
            }
        }

        private void chkUserCaptureSQL_CheckedChanged(object sender, EventArgs e)
        {
            // SQLCM 5563: Disable Logout with Logins for Server settings not properly configured while applying Regulation Guideline at Server Level
            if (!_templateSvrInitializationCompleted || !this._templateDBInitializationCompleted)
            {
                return;
            }
            if (chkUserAuditCaptureSQL.Checked &&
                _currentPage != WizardPage.PrivilegedUsers)
            {
                //the message shouldn't be showed at privilege users step
                ErrorMessage.Show(this.Text, UIConstants.Warning_CaptureAll, "", MessageBoxIcon.Warning);
            }
        }
        //start sqlcm 5.6 - 5464
        private void chkUserCaptureSQLDatabase_CheckedChanged(object sender, EventArgs e)
        {
            // SQLCM 5563: Disable Logout with Logins for Server settings not properly configured while applying Regulation Guideline at Server Level
            if (_templateSvrInitializationCompleted && chkUserAuditCaptureSQLDatabase.Checked &&
                _currentPage != WizardPage.PrivilegedUsersDatabase)
            {
                //the message shouldn't be showed at privilege users step
                ErrorMessage.Show(this.Text, UIConstants.Warning_CaptureAll, "", MessageBoxIcon.Warning);
            }
        }

        private void Click_chkUserAccessCheckFilterDatabase(object sender, EventArgs e)
        {
            if (chkUserAccessCheckFilterDatabase.Checked)
            {
                rbUserAuditFailedOnlyDatabase.Enabled = true;
                rbUserAuditSuccessfulOnlyDatabase.Enabled = true;
            }
            else
            {
                rbUserAuditFailedOnlyDatabase.Enabled = false;
                rbUserAuditSuccessfulOnlyDatabase.Enabled = false;
            }
        }
        //start sqlcm 5.6 - 5464

        private void rbAuditUserSelectedDatabase_CheckedChanged(object sender, EventArgs e)
        {
            grpAuditUserActivityDatabase.Enabled = rbAuditUserSelectedDatabase.Checked;
        }

        private void chkAuditUserDMLDatabase_CheckedChanged(object sender, EventArgs e)
        {
            //DML only setting
            if (rbAuditUserSelectedDatabase.Checked && chkUserAuditDMLDatabase.Checked)
                chkUserAuditCaptureTransDatabase.Enabled = true;
            else
            {
                chkUserAuditCaptureTransDatabase.Checked = false;
                chkUserAuditCaptureTransDatabase.Enabled = false;
            }

            //DML or SELECT Setting
            if (rbAuditUserSelectedDatabase.Checked && (chkUserAuditDMLDatabase.Checked || chkUserAuditSELECTDatabase.Checked) && CoreConstants.AllowCaptureSql)
                chkUserAuditCaptureSQLDatabase.Enabled = true;
            else
            {
                chkUserAuditCaptureSQLDatabase.Checked = false;
                chkUserAuditCaptureSQLDatabase.Enabled = false;
            }

            //DDL Setting
            if (rbAuditUserSelectedDatabase.Checked && chkUserAuditDDLDatabase.Checked && CoreConstants.AllowCaptureSql)
                chkUserCaptureDDLDatabase.Enabled = true;
            else
            {
                chkUserCaptureDDLDatabase.Checked = false;
                chkUserCaptureDDLDatabase.Enabled = false;
            }
        }
        //end sqlcm 5.6 - 5464
        #endregion

        #region Database Events
        private void Click_chkFilterOnAccess(object sender, EventArgs e)
        {
            if (chkDBAccessCheckFilter.Checked)
            {
                rbDBAuditFailedOnly.Enabled = true;
                rbDBAuditSuccessfulOnly.Enabled = true;
            }
            else
            {
                rbDBAuditFailedOnly.Enabled = false;
                rbDBAuditSuccessfulOnly.Enabled = false;
            }
        }

        private void chkAuditDML_CheckedChanged(object sender, EventArgs e)
        {
            if ((chkDBAuditDML.Checked || chkDBAuditSELECT.Checked) && CoreConstants.AllowCaptureSql)
                chkDBAuditCaptureSQL.Enabled = true;
            else
                chkDBAuditCaptureSQL.Enabled = false;

            if (chkDBAuditDML.Checked && ServerRecord.CompareVersions(_server.AgentVersion, "3.5") >= 0)
                chkDBAuditCaptureTrans.Enabled = true;
            else
                chkDBAuditCaptureTrans.Enabled = false;
        }

        private void chkCaptureSQL_CheckedChanged(object sender, EventArgs e)
        {
            if (chkDBAuditCaptureSQL.Checked)
            {
                ErrorMessage.Show(this.Text, UIConstants.Warning_CaptureAll, "", MessageBoxIcon.Warning);
            }
        }
        #endregion

        #region DMLFilters

        private void radioAllDML_CheckedChanged(object sender, EventArgs e)
        {
            SetFilterState();
        }

        private void SetFilterState()
        {
            chkAuditUserTables.Enabled = radioSelectedDML.Checked;
            chkAuditSystemTables.Enabled = radioSelectedDML.Checked;
            chkAuditStoredProcedures.Enabled = radioSelectedDML.Checked;
            chkAuditOther.Enabled = radioSelectedDML.Checked;
        }

        #endregion

        #region Trusted Users
        private void btnAddTrusted_Click(object sender, EventArgs e)
        {
            Form_PrivUser frm = new Form_PrivUser(_server.Instance, false);

            frm.UseAgentEnumeration = _server.IsDeployed;

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
                    btnRemoveTrusted.Enabled = true;
                }
            }
        }

        private void btnRemoveTrusted_Click(object sender, EventArgs e)
        {
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
                btnRemoveTrusted.Enabled = false;
            }
        }

        #endregion
        private void lstPrivilegedUsers_SelectedIndexChanged(object sender, System.EventArgs e)
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

        /// <summary>
        /// Update for Selections in the privilege users for the databse level
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lstPrivilegedUsersDb_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            // SQLCM-5375 Greying logic for privileged users
            this.lstPrivilegedUsersdb.SelectedIndexChanged -= new System.EventHandler(this.lstPrivilegedUsersDb_SelectedIndexChanged);

            foreach (ListViewItem priv in lstPrivilegedUsersdb.SelectedItems)
            {
                if (priv.ForeColor == System.Drawing.Color.Gray)
                {
                    priv.Selected = false;
                }
            }
            this.lstPrivilegedUsersdb.SelectedIndexChanged += new System.EventHandler(this.lstPrivilegedUsersDb_SelectedIndexChanged);

            if (lstPrivilegedUsersdb.SelectedItems.Count == 0)
            {
                btnRemovePrivdb.Enabled = false;
            }
            else
            {
                btnRemovePrivdb.Enabled = Globals.isAdmin;
            }
        }

        //SQLCM-5658
        private void SelectedIndexChanged_lstTrustedUsers(object sender, EventArgs e)
        {
            if (lstTrustedUsers.SelectedItems.Count == 0)
            {
                btnRemoveTrusted.Enabled = false;
            }
            else
            {
                btnRemoveTrusted.Enabled = Globals.isAdmin;
                // v5.6 SQLCM-5373 - disable remove button if the trusted user is set at server level
                UserList userList = new UserList(_server.AuditTrustedUsersList);
                string selectedText = lstTrustedUsers.SelectedItems[0].Text;

                foreach (Login l in userList.Logins)
                {
                    if (selectedText.Equals(l.Name))
                    {
                        btnRemoveTrusted.Enabled = false;
                        break;
                    }
                }
                foreach (ServerRole l in userList.ServerRoles)
                {
                    if (selectedText.Equals(l.Name))
                    {
                        btnRemoveTrusted.Enabled = false;
                        break;
                    }
                }

                foreach (ListViewItem priv in lstTrustedUsers.SelectedItems)
                {
                    if (priv.ForeColor == System.Drawing.Color.Gray)
                    {
                        priv.Selected = false;
                    }
                }
            }
        }
        //SQLCM-5658 v5.6- END
        // v5.6 SQLCM-5373
        private void btnAddServerTrusted_Click(object sender, EventArgs e)
        {
            Form_PrivUser frm = new Form_PrivUser(_server.Instance, false);

            frm.UseAgentEnumeration = _server.IsDeployed;

            //frm.MainForm = this.mainForm;                                                      
            if (DialogResult.OK == frm.ShowDialog())
            {
                lstServerTrustedUsers.BeginUpdate();

                lstServerTrustedUsers.SelectedItems.Clear();

                foreach (ListViewItem itm in frm.listSelected.Items)
                {
                    bool found = false;
                    foreach (ListViewItem s in lstServerTrustedUsers.Items)
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
                        lstServerTrustedUsers.Items.Add(newItem);
                    }
                }

                lstServerTrustedUsers.EndUpdate();

                if (lstServerTrustedUsers.Items.Count > 0)
                {
                    lstServerTrustedUsers.TopItem.Selected = true;
                    btnRemoveServerTrusted.Enabled = true;
                }
            }
        }

        private void btnRemoveServerTrusted_Click(object sender, EventArgs e)
        {
            lstServerTrustedUsers.BeginUpdate();

            int ndx = lstServerTrustedUsers.SelectedIndices[0];

            foreach (ListViewItem priv in lstServerTrustedUsers.SelectedItems)
            {
                priv.Remove();
            }

            lstServerTrustedUsers.EndUpdate();

            // reset selected item
            if (lstServerTrustedUsers.Items.Count != 0)
            {
                lstServerTrustedUsers.Focus();
                if (ndx >= lstServerTrustedUsers.Items.Count)
                {
                    lstServerTrustedUsers.Items[lstServerTrustedUsers.Items.Count - 1].Selected = true;
                }
                else
                    lstServerTrustedUsers.Items[ndx].Selected = true;
            }
            else
            {
                btnRemoveServerTrusted.Enabled = false;
            }
        }

        #region Regulation
        private void checkPCI_CheckedChanged(object sender, EventArgs e)
        {
            if (checkPCI.Checked)
            {
                applyRegulationSetting(Regulation.RegulationTypeUser.PCI);
            }
            else
            {
                removeRegulationSetting(Regulation.RegulationTypeUser.PCI);
            }
            UpdateNext();
        }

        private void checkHIPAA_CheckedChanged(object sender, EventArgs e)
        {
            if (checkHIPAA.Checked)
            {
                applyRegulationSetting(Regulation.RegulationTypeUser.HIPAA);
            }
            else
            {
                removeRegulationSetting(Regulation.RegulationTypeUser.HIPAA);
            }
            UpdateNext();
        }

        private void checkGDPR_CheckedChanged(object sender, System.EventArgs e)
        {
            if (checkGDPR.Checked)
            {
                applyRegulationSetting(Regulation.RegulationTypeUser.GDPR);
            }
            else
            {
                removeRegulationSetting(Regulation.RegulationTypeUser.GDPR);
            }
            UpdateNext();
        }

        private void checkCIS_CheckedChanged(object sender, EventArgs e)
        {
            if (checkCIS.Checked)
            {
                applyRegulationSetting(Regulation.RegulationTypeUser.CIS);
            }
            else
            {
                removeRegulationSetting(Regulation.RegulationTypeUser.CIS);
            }
            UpdateNext();
        }

        private void checkDISA_CheckedChanged(object sender, EventArgs e)
        {
            if (checkDISA.Checked)
            {
                applyRegulationSetting(Regulation.RegulationTypeUser.DISA);
            }
            else
            {
                removeRegulationSetting(Regulation.RegulationTypeUser.DISA);
            }
            UpdateNext();
        }

        private void checkNERC_CheckedChanged(object sender, EventArgs e)
        {
            if (checkNERC.Checked)
            {
                applyRegulationSetting(Regulation.RegulationTypeUser.NERC);
            }
            else
            {
                removeRegulationSetting(Regulation.RegulationTypeUser.NERC);
            }
            UpdateNext();
        }

        private void checkSOX_CheckedChanged(object sender, EventArgs e)
        {
            if (checkSOX.Checked)
            {
                applyRegulationSetting(Regulation.RegulationTypeUser.SOX);
            }
            else
            {
                removeRegulationSetting(Regulation.RegulationTypeUser.SOX);
            }
            UpdateNext();
        }

        private void checkFERPA_CheckedChanged(object sender, EventArgs e)
        {
            if (checkFERPA.Checked)
            {
                applyRegulationSetting(Regulation.RegulationTypeUser.FERPA);
            }
            else
            {
                removeRegulationSetting(Regulation.RegulationTypeUser.FERPA);
            }
            UpdateNext();
        }

        private void UpdateNext()
        {
            // Consider greying logic for regulation settings
            if (((checkCustom.Enabled && checkCustom.Checked)
                || (updatedSettings != oldSettings))
                && updatedSettings > 0)
                nextButton.Enabled = true;
            else
                nextButton.Enabled = false;
        }

        //Apply corresponding regulation setting in the updatedSettings
        private void applyRegulationSetting(Regulation.RegulationTypeUser regulationType)
        {
            updatedSettings = updatedSettings | (int)regulationType;
        }

        //Remove corresponding regulation setting in the updatedSettings
        private void removeRegulationSetting(Regulation.RegulationTypeUser regulationType)
        {
            updatedSettings = updatedSettings & ~(int)regulationType;
        }

        //Checks whether a particular regulation guideline is applied or not
        private bool checkRegulationSetting(Regulation.RegulationTypeUser regulationType)
        {
            if ((oldSettings & (int)regulationType) > 0)
            {
                return true;
            }

            return false;
        }

        //Fix for 5241
        //Load current user applied regulation settings from DB
        private void LoadRegulationSettings()
        {
            // Minor Fix: Regulation Guidelines for new server shows up existing guideline selection for the existing server
            if (Globals.isServerNodeSelected)
            {
                // Use Current Server Id if present over the currentServerId
                oldSettings = RegulationSettings.LoadUserAppliedSettingsServer(
                    Globals.Repository.Connection,
                    Server != null ? Server.SrvId : CoreHelpers.currentServerId);
            }
            else
            {
                // Use Current Server Id if present over the currentServerId
                oldSettings = RegulationSettings.LoadUserAppliedSettingsDatabase(
                    Globals.Repository.Connection,
                    Server != null ? Server.SrvId : CoreHelpers.currentServerId,
                    CoreHelpers.currentDatabaseId);
            }

            if (checkRegulationSetting(Regulation.RegulationTypeUser.PCI))
            {
                checkPCI.Checked = true;
            }

            if (checkRegulationSetting(Regulation.RegulationTypeUser.HIPAA))
            {
                checkHIPAA.Checked = true;
            }

            if (checkRegulationSetting(Regulation.RegulationTypeUser.GDPR))
            {
                checkGDPR.Checked = true;
            }

            if (checkRegulationSetting(Regulation.RegulationTypeUser.CIS))
            {
                checkCIS.Checked = true;
            }

            if (checkRegulationSetting(Regulation.RegulationTypeUser.DISA))
            {
                checkDISA.Checked = true;
            }

            if (checkRegulationSetting(Regulation.RegulationTypeUser.NERC))
            {
                checkNERC.Checked = true;
            }

            if (checkRegulationSetting(Regulation.RegulationTypeUser.SOX))
            {
                checkSOX.Checked = true;
            }

            if (checkRegulationSetting(Regulation.RegulationTypeUser.FERPA))
            {
                checkFERPA.Checked = true;
            }

        }

        private void LoadRegulations()
        {
            try
            {
                _regulations = RegulationDAL.LoadRegulations(Globals.Repository.Connection);

                foreach (Regulation regulation in _regulations)
                {
                    switch (regulation.RegType)
                    {
                        case Regulation.RegulationType.PCI:
                            {
                                //_pciDescription = regulation.Description;
                                checkPCI.Text = regulation.Name;
                                break;
                            }
                        case Regulation.RegulationType.HIPAA:
                            {
                                //_hipaaDescription = regulation.Description;
                                checkHIPAA.Text = regulation.Name;
                                break;
                            }
                        case Regulation.RegulationType.GDPR:
                            {
                                checkGDPR.Text = regulation.Name;
                                break;
                            }
                        case Regulation.RegulationType.CIS:
                            {
                                //_hipaaDescription = regulation.Description;
                                checkCIS.Text = regulation.Name;
                                break;
                            }
                        case Regulation.RegulationType.DISA:
                            {
                                //_hipaaDescription = regulation.Description;
                                checkDISA.Text = regulation.Name;
                                break;
                            }
                        case Regulation.RegulationType.NERC:
                            {
                                //_hipaaDescription = regulation.Description;
                                checkNERC.Text = regulation.Name;
                                break;
                            }
                        case Regulation.RegulationType.SOX:
                            {
                                //_hipaaDescription = regulation.Description;
                                checkSOX.Text = regulation.Name;
                                break;
                            }
                        case Regulation.RegulationType.FERPA:
                            {
                                // _hipaaDescription = regulation.Description;
                                checkFERPA.Text = regulation.Name;
                                break;
                            }
                    }
                }
            }
            catch (Exception e)
            {
                // _currentPage = WizardPage.UnableToLoadRegulations;
                return;
            }
            LoadRegulationCategories();
        }

        private void LoadRegulationCategories()
        {
            try
            {
                _regulationSettings = RegulationDAL.LoadRegulationCategories(Globals.Repository.Connection);
            }
            catch (Exception e)
            {
                // _currentPage = WizardPage.UnableToLoadRegulations;
            }
        }

        #endregion

        #region SensitiveColumns

        private void InitSensitiveColumns()
        {
            TreeNode root = new TreeNode();
            TreeNode node;
            RawDatabaseObject rdo;
            root.Text = _server.Instance;
            treeDatabases.Nodes.Clear();
            //listViewTables.Items.Clear();

            if (_startPage == StartPage.RegulationGuidelineInfo)
            {
                foreach (DatabaseRecord db in _databases)
                {
                    node = new TreeNode();
                    rdo = new RawDatabaseObject();
                    node.Text = rdo.name = db.Name;
                    node.Tag = rdo;
                    root.Nodes.Add(node);
                    LoadSensitiveColumns(db);
                }
            }
            else
            {
                foreach (RawDatabaseObject db in listDatabases.CheckedItems)
                {
                    node = new TreeNode();
                    node.Text = db.name;
                    node.Tag = db;
                    root.Nodes.Add(node);
                }
            }
            treeDatabases.Nodes.Add(root);
            treeDatabases.ExpandAll();
            treeDatabases.SelectedNode = root.Nodes[0];
            _loadedSCTables = true;
        }

        private bool SupportsSensitiveColumns()
        {

            //SQLcm 5.6 - Fix for 5820
            if (_server != null && _server.AgentVersion.Trim() == "")
            {
                IList listDatabaseDetails;
                listDatabaseDetails = ServerRecord.GetAgentVersionDataForServerId(Globals.Repository.Connection, _server.SrvId);
                if (listDatabaseDetails != null && listDatabaseDetails.Count > 0)
                    foreach (Idera.SQLcompliance.Core.Agent.ServerAgentDetails sd in listDatabaseDetails)
                    {
                        _server.AgentVersion = sd.AgentVersion;
                        _server.SqlVersion = sd.SqlVersion;
                    }
            }

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

        private void LoadSensitiveColumns(DatabaseRecord db)
        {
            if (db.AuditSensitiveColumns)
            {
                if (_loadedSCTables)
                    return;

                if (!LoadTables(db.Name))
                { return; }
                List<string> missingTables = new List<string>();
                List<SensitiveColumnTableRecord> scTables = SensitiveColumnTableRecord.GetAuditedTables(Globals.Repository.Connection, _server.SrvId, db.DbId);
                string tableName;
                if (scTables != null)
                {
                    foreach (SensitiveColumnTableRecord table in scTables)
                    {
                        if (SupportsSchemas())
                        {
                            tableName = table.FullTableName;
                        }
                        else
                            tableName = table.TableName;

                        ListViewItem x = _lvSCTables.Items.Add(tableName);
                        x.SubItems.Add(table.SelectedColumns ? Form_TableConfigure.GetColumnString(table.Columns) : UIConstants.SC_AllColumns);
                        x.SubItems.Add(table.Type);

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
                        // reset selected item
                        {
                            missingTables.Add(table.FullTableName);
                            x.Tag = null;
                            x.ForeColor = System.Drawing.Color.LightGray;
                        }
                        else
                            x.Tag = _tableObjects[tableName];
                    }
                    _dictLvSCTables[db.Name] = _lvSCTables;
                }
            }
        }

        private bool LoadTables(string dbName)
        {
            // Attempt to load list of tables if we haven't tried already
            if (_tableList == null && _tableObjects == null)
            {
                _tableObjects = new Dictionary<string, DatabaseObjectRecord>();
                // try via connection to agent
                if (_server.IsDeployed && _server.IsRunning)
                {
                    string url = EndPointUrlBuilder.GetUrl(typeof(AgentManager), Globals.SQLcomplianceConfig.Server, Globals.SQLcomplianceConfig.ServerPort);
                    try
                    {
                        AgentManager manager = GUIRemoteObjectsProvider.AgentManager();
                        _tableList = manager.GetRawTables(_server.Instance, dbName);
                    }
                    catch (Exception ex)
                    {
                        ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                                String.Format("LoadTables: URL: {0} Instance {1} Database {2}", url, _server.Instance, dbName),
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
                        _tableList = RawSQL.GetTables(_sqlServer.Connection, dbName);
                    }
                }
                bool supportsSchemas = SupportsSchemas();

                if (_tableList != null)
                {
                    foreach (RawTableObject rto in _tableList)
                    {
                        DatabaseObjectRecord dbo = new DatabaseObjectRecord(rto);
                        if (supportsSchemas)
                        {
                            //_tableObjects.Add(dbo.FullTableName, dbo);
                            //start sqlcm 5.6 - 5671
                            if (dbo.TableName.StartsWith(dbo.SchemaName))
                                _tableObjects.Add(dbo.SchemaName + "." + dbo.TableName, dbo);
                            //end sqlcm 5.6 - 5671
                            else
                                _tableObjects.Add(dbo.FullTableName, dbo);
                        }
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

            if (String.IsNullOrEmpty(_server.AgentVersion) ||
               _server.AgentVersion.StartsWith("1") ||
               _server.AgentVersion.StartsWith("2") ||
               _server.AgentVersion.StartsWith("3.0"))
                return false;
            else
                return true;
        }

        private void listViewTables_SelectedIndexChanged(object sender, EventArgs e)
        {
            //if (listViewTables.SelectedItems.Count == 0)
            //   buttonRemoveTable.Enabled = false;
            //else
            //   buttonRemoveTable.Enabled = true;
        }

        private void treeDatabases_BeforeSelect(object sender, TreeViewCancelEventArgs e)
        {
            //listViewTables.Items.Clear();
            //_tableList = _tableObjects = null;
            //buttonAddTable.Enabled = true;
        }

        private void treeDatabases_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (treeDatabases.SelectedNode.Parent == null)
            {
                treeDatabases.SelectedNode = treeDatabases.SelectedNode.Nodes[0];
            }
            RawDatabaseObject db = (RawDatabaseObject)treeDatabases.SelectedNode.Tag;
            if (_startPage == StartPage.RegulationGuidelineInfo)
            {
                foreach (var dbRecord in _databases)
                {
                    if (db.name == dbRecord.Name)
                    {
                        _dbSCTreeSelection = dbRecord;
                        break;
                    }
                }
            }
            else
            {
                _dbSCTreeSelection = new DatabaseRecord();
                _dbSCTreeSelection.Name = db.name;
            }
        }
        #endregion

        #region Summary

        private void LoadSummaryInfo()
        {
            StringBuilder builder = new StringBuilder();

            labelSummaryInstance.Text = _server.Instance;

            if (_startPage == StartPage.RegulationGuidelineInfo)
            {
                foreach (DatabaseRecord db in _databases)
                {
                    builder.AppendFormat("{0}\r\n", db.Name);
                }
            }
            else
            {
                foreach (RawDatabaseObject rdo in listDatabases.CheckedItems)
                {
                    builder.AppendFormat("{0}\r\n", rdo.name);
                }
            }
            textSummaryDatabases.Text = builder.ToString();

            if (_auditType == AuditingType.Regulation)
            {
                linkSummaryDetails.Visible = true;

                builder = new StringBuilder();

                if (checkPCI.Checked)
                {
                    builder.Append("PCI");
                }
                if (checkHIPAA.Checked)
                {
                    if (builder.Length > 0)
                        builder.Append(", HIPAA ");
                    else
                        builder.Append("HIPAA ");
                }
                if (checkGDPR.Checked)
                {
                    if (builder.Length > 0)
                        builder.Append(", GDPR ");
                    else
                        builder.Append("GDPR ");
                }
                if (checkDISA.Checked)
                {
                    if (builder.Length > 0)
                        builder.Append(", DISA ");
                    else
                        builder.Append("DISA ");
                }
                if (checkNERC.Checked)
                {
                    if (builder.Length > 0)
                        builder.Append(", NERC ");
                    else
                        builder.Append("NERC ");
                }
                if (checkCIS.Checked)
                {
                    if (builder.Length > 0)
                        builder.Append(", CIS ");
                    else
                        builder.Append("CIS ");
                }
                if (checkSOX.Checked)
                {
                    if (builder.Length > 0)
                        builder.Append(", SOX ");
                    else
                        builder.Append("SOX ");
                }
                if (checkFERPA.Checked)
                {
                    if (builder.Length > 0)
                        builder.Append(", FERPA ");
                    else
                        builder.Append("FERPA ");
                }
                if (_isCustomTemplate || checkCustom.Checked)
                {
                    builder.Clear();
                    builder.Append("Custom");
                }

                labelSummaryAuditLevel.Text = builder.ToString();
            }
            else
            {
                labelSummaryAuditLevel.Text = _auditType.ToString();
                linkSummaryDetails.Visible = false;
            }
        }

        private void linkSummaryDetails_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Form_RegulationDetails details = new Form_RegulationDetails(checkPCI.Checked, checkHIPAA.Checked, checkDISA.Checked, checkNERC.Checked, checkCIS.Checked, checkSOX.Checked, checkFERPA.Checked, checkGDPR.Checked);
            details.ShowDialog(this);
        }

        private bool CheckDBExists(string srvName, string dbName)
        {
            int dbId = -1;
            dbId = DatabaseRecord.GetDatabaseId(Globals.Repository.Connection, srvName, dbName);
            return dbId > 0 ? true : false;
        }

        private void Save()
        {
            //Update Audit Settings for the instance
            if (_startPage == StartPage.Server || (_startPage == StartPage.RegulationGuidelineInfo && Globals.isServerNodeSelected))
            {
                SaveInstanceAuditSettings();
            }

            //Add Selected databases, if any
            SaveDatabaseRecords();

            if (_startPage == StartPage.Server)
            {
                ServerRecord sr = new ServerRecord();
                string result = sr.IsInstalledManually(_server.Instance);
                if (result != null)
                {
                    EventDatabase.UpdateManuallyDeployed(this.textSQLServer.Text, "2", Globals.Repository.Connection);
                }
            }
        }

        private bool SaveInstanceAuditSettings()
        {
            string errorMsg = "";
            bool retval = false;
            bool isDirty = false;
            ServerRecord newServer;

            if (ValidatePrivilegedUsers())
            {
                newServer = CreateModifiedServerRecord();
                newServer.Connection = Globals.Repository.Connection;
                if (_auditType == AuditingType.Default)
                {
                    ApplyDefaultServerSettings(newServer);
                    UpdateServerTraceOrXESetting(false, newServer);
                }
                try
                {
                    //---------------------------------------
                    // Write Server Properties if necessary
                    //---------------------------------------
                    if (!ServerRecord.Match(newServer, _server))
                    {
                        if (!newServer.Write(_server))
                        {
                            errorMsg = ServerRecord.GetLastError();
                            throw (new Exception(errorMsg));
                        }
                        else
                        {
                            if (_auditType == AuditingType.Default)
                                UpdateThresholds(newServer.SrvId);
                            // update default security
                            if (newServer.DefaultAccess != _server.DefaultAccess)
                            {
                                EventDatabase.SetDefaultSecurity(_server.EventDatabase,
                                                                 newServer.DefaultAccess,
                                                                 _server.DefaultAccess,
                                                                 false,
                                                                 Globals.Repository.Connection);
                            }

                            isDirty = true;
                        }
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
                    if (retval && isDirty && _server.IsAuditedServer)
                    {
                        string changeLog = Snapshot.ServerChangeLog(_server, newServer);

                        // Register change to server and perform audit log				      
                        ServerUpdate.RegisterChange(newServer.SrvId,
                                                    LogType.ModifyServer,
                                                    newServer.Instance,
                                                    changeLog);

                        // in case agent properties were updated, update all instances
                        //newServer.CopyAgentSettingsToAll(oldServer);

                        newServer.ConfigVersion++;
                        _server = newServer;
                    }
                    //SQLCM 5.7 Requirement 5.3.4.2
                    IFormatter formatter;
                    MemoryStream streamTrustedUsers = null;
                    MemoryStream streamPrivilegedUsers = null;
                    streamTrustedUsers = new MemoryStream(Convert.FromBase64String(newServer.AuditTrustedUsersList));
                    streamPrivilegedUsers = new MemoryStream(Convert.FromBase64String(newServer.AuditUsersList));
                    formatter = new BinaryFormatter();
                    RemoteUserList trustedUserList = new RemoteUserList();
                    RemoteUserList privilegedUserList = new RemoteUserList();
                    if (newServer.AuditTrustedUsersList != null && newServer.AuditTrustedUsersList != "")
                    {
                        trustedUserList = (RemoteUserList)formatter.Deserialize(streamTrustedUsers);
                    }
                    if (newServer.AuditUsersList != null && newServer.AuditUsersList != "")
                    {
                        privilegedUserList = (RemoteUserList)formatter.Deserialize(streamPrivilegedUsers);
                    }
                    var error = string.Empty;
                    try
                    {
                        if (!newServer.SaveServerLevelUsersFromWizard(newServer, trustedUserList, privilegedUserList))
                        {
                            error = ServerRecord.GetLastError();
                            throw (new Exception(error));
                        }
                    }
                    catch (Exception ex)
                    {
                        error = ex.Message;
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

        private void ApplyDefaultServerSettings(ServerRecord srv)
        {
            var cmdstr = string.Format("select * from {0}..{1}", CoreConstants.RepositoryDatabase, CoreConstants.DefaultServerPropertise);

            using (SqlCommand cmd = new SqlCommand(cmdstr, Globals.Repository.Connection))
            {
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        UserList defaultPrivilegedUserList = new UserList(SQLHelpers.GetString(reader, reader.GetOrdinal("auditUsersList")));
                        UserList defaultTrustedUserList = new UserList(SQLHelpers.GetString(reader, reader.GetOrdinal("auditTrustedUsersList")));

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

                        srv.AuditLogins = auditLogins;
                        srv.AuditLogouts = auditLogouts;
                        srv.AuditFailedLogins = auditFailedLogins;
                        srv.AuditDDL = auditDDL;
                        srv.AuditSecurity = auditSecurity;
                        srv.AuditAdmin = auditAdmin;
                        srv.AuditUDE = auditUDE;
                        srv.AuditTrace = auditTrace;
                        srv.AuditCaptureSQLXE = auditCaptureSQLXE;
                        srv.IsAuditLogEnabled = isAuditLogEnabled;
                        srv.AuditAccessCheck = (AccessCheckFilter)auditFailures;
                        srv.AuditUserAll = auditUserAll;
                        srv.AuditUserLogins = auditUserLogins;
                        srv.AuditUserLogouts = auditUserLogouts;
                        srv.AuditUserFailedLogins = auditUserFailedLogins;
                        srv.AuditUserDDL = auditUserDDL;
                        srv.AuditUserSecurity = auditUserSecurity;
                        srv.AuditUserAdmin = auditUserAdmin;
                        srv.AuditUserDML = auditUserDML;
                        srv.AuditUserSELECT = auditUserSELECT;
                        srv.AuditUserUDE = auditUserUDE;
                        srv.AuditUserAccessCheck = (AccessCheckFilter)auditUserFailures;
                        srv.AuditUserCaptureSQL = auditUserCaptureSQL;
                        srv.AuditUserCaptureTrans = auditUserCaptureTrans;
                        srv.AuditUserCaptureDDL = auditUserCaptureDDL;
                        srv.DefaultAccess = defaultAccess;
                        srv.MaxSqlLength = maxSqlLength;
                        #region
                        var m_useAgentEnum = false;
                        ICollection roleList = null;
                        ICollection loginList = null;
                        SQLDirect sqlServer = null;

                        m_useAgentEnum = srv.IsDeployed && srv.IsRunning;

                        if (m_useAgentEnum)
                        {
                            string url = EndPointUrlBuilder.GetUrl(typeof(AgentManager), Globals.SQLcomplianceConfig.Server, Globals.SQLcomplianceConfig.ServerPort);
                            try
                            {
                                AgentManager manager = GUIRemoteObjectsProvider.AgentManager();


                                roleList = manager.GetRawServerRoles(srv.Instance);

                                loginList = manager.GetRawServerLogins(srv.Instance);

                            }
                            catch (Exception ex)
                            {
                                ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                                         String.Format("LoadRoles or Logins: URL: {0} Instance {1}", url, srv.Instance),
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
                            if (sqlServer.OpenConnection(srv.Instance))
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

                        srv.AuditTrustedUsersList = formattedTrustedUserList;
                        srv.AuditUsersList = formattedPrvUserList;
                    }
                    reader.Close();
                }
            }
        }

        private void UpdateThresholds(int serverId)
        {
            var records = ReportCardRecord.GetDefaultServerReportCardEntries(Globals.Repository.Connection, false);
            foreach (var record in records)
                record.SrvId = serverId;

            var existingRecords = ReportCardRecord.GetServerReportCardEntries(Globals.Repository.Connection, serverId);
            var existingStatIds = existingRecords.Select(erecord => erecord.StatisticId);
            var recordsToUpdate = records.Where(record => existingStatIds.Contains(record.StatisticId));
            var recordsToInsert = records.Except(recordsToUpdate);

            foreach (var updateRecord in recordsToUpdate)
                updateRecord.Update(Globals.Repository.Connection);

            foreach (var insertRecord in recordsToInsert)
                insertRecord.Write(Globals.Repository.Connection);

        }
        private ServerRecord CreateModifiedServerRecord()
        {
            ServerRecord srv = _server.Clone();

            // default access
            if (radioGrantAll.Checked)
                srv.DefaultAccess = 2;
            else if (radioGrantEventsOnly.Checked)
                srv.DefaultAccess = 1;
            else
                srv.DefaultAccess = 0;

            if (srv.IsAuditedServer)
            {
                if (_auditType == AuditingType.Regulation)
                    ApplyServerRegulation(srv);
                else
                {
                    // Audit Settings		
                    srv.AuditLogins = chkServerAuditLogins.Checked;
                    srv.AuditLogouts = chkServerAuditLogouts.Checked;
                    srv.AuditFailedLogins = chkServerAuditFailedLogins.Checked;
                    srv.AuditDDL = chkServerAuditDDL.Checked;
                    srv.AuditAdmin = chkServerAuditAdmin.Checked;
                    srv.AuditSecurity = chkServerAuditSecurity.Checked;
                    srv.AuditUDE = chkServerAuditUDE.Checked;
                    if (chkServerAccessCheckFilter.Checked)
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
                }
                // Privileged users
                srv.AuditUsersList = GetPrivilegedUserProperty();
                //Server LevelTrusted Users // v5.6 SQLCM-5373
                srv.AuditTrustedUsersList = GetServerLevelTrustedUserProperty();
                srv.AuditUserAll = rbAuditUserAll.Checked;
                srv.AuditUserLogins = chkUserAuditLogins.Checked;
                srv.AuditUserLogouts = chkUserAuditLogouts.Checked;
                srv.AuditUserFailedLogins = chkUserAuditFailedLogins.Checked;
                srv.AuditUserDDL = chkUserAuditDDL.Checked;
                srv.AuditUserSecurity = chkUserAuditSecurity.Checked;
                srv.AuditUserAdmin = chkUserAuditAdmin.Checked;
                srv.AuditUserDML = chkUserAuditDML.Checked;
                srv.AuditUserSELECT = chkUserAuditSELECT.Checked;
                srv.AuditUserUDE = chkUserAuditUDE.Checked;
                if (chkUserAccessCheckFilter.Checked)
                {
                    if (rbUserAuditFailedOnly.Checked)
                        srv.AuditUserAccessCheck = AccessCheckFilter.FailureOnly;
                    else
                        srv.AuditUserAccessCheck = AccessCheckFilter.SuccessOnly;
                }
                else
                {
                    srv.AuditUserAccessCheck = AccessCheckFilter.NoFilter;
                }
                srv.AuditUserCaptureSQL = chkUserAuditCaptureSQL.Enabled && chkUserAuditCaptureSQL.Checked;
                srv.AuditUserCaptureTrans = chkUserAuditCaptureTrans.Enabled && chkUserAuditCaptureTrans.Checked;
                srv.AuditUserCaptureDDL = chkUserCaptureDDL.Enabled && chkUserCaptureDDL.Checked;
            }
            return srv;
        }

        private void ApplyServerRegulation(ServerRecord srv)
        {
            ServerRecord tempServer = new ServerRecord();
            RegulationSettings settings;

            if (_isCustomTemplate || checkCustom.Checked)
            {
                tempServer.AuditLogins = _checkLogins.Checked;
                tempServer.AuditLogouts = _checkLogouts.Checked;
                tempServer.AuditFailedLogins = _checkFailedLogins.Checked;
                tempServer.AuditDDL = _checkDDL.Checked;
                tempServer.AuditAdmin = _checkAdministrativeActivities.Checked;
                tempServer.AuditSecurity = _checkSecurityChanges.Checked;
                tempServer.AuditUDE = _checkUserDefinedEvents.Checked;

                if (_chkServerAccessCheckFilter.Checked)
                {
                    if (_rbAuditFailedOnly.Checked)
                        tempServer.AuditUserAccessCheck = AccessCheckFilter.FailureOnly;
                    else
                        tempServer.AuditUserAccessCheck = AccessCheckFilter.SuccessOnly;
                }
                else
                {
                    tempServer.AuditAccessCheck = AccessCheckFilter.NoFilter;
                }
            }
            else
            {
                // apply PCI
                if (checkPCI.Checked)
                {
                    if (_regulationSettings.TryGetValue((int)Regulation.RegulationType.PCI, out settings))
                    {
                        tempServer.AuditLogins = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.Logins) == (int)RegulationSettings.RegulationServerCategory.Logins) || tempServer.AuditLogins;
                        tempServer.AuditLogouts = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.Logouts) == (int)RegulationSettings.RegulationServerCategory.Logouts) || tempServer.AuditLogouts;
                        tempServer.AuditFailedLogins = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.FailedLogins) == (int)RegulationSettings.RegulationServerCategory.FailedLogins) || tempServer.AuditFailedLogins;
                        tempServer.AuditDDL = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.DatabaseDefinition) == (int)RegulationSettings.RegulationServerCategory.DatabaseDefinition) || tempServer.AuditDDL;
                        tempServer.AuditAdmin = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.AdminActivity) == (int)RegulationSettings.RegulationServerCategory.AdminActivity) || tempServer.AuditAdmin;
                        tempServer.AuditSecurity = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.SecurityChanges) == (int)RegulationSettings.RegulationServerCategory.SecurityChanges) || tempServer.AuditSecurity;
                        tempServer.AuditUDE = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.UserDefined) == (int)RegulationSettings.RegulationServerCategory.UserDefined) || tempServer.AuditUDE;

                        if (((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.FilterPassedAccess) == (int)RegulationSettings.RegulationServerCategory.FilterPassedAccess) || (tempServer.AuditAccessCheck == AccessCheckFilter.SuccessOnly))
                            tempServer.AuditAccessCheck = AccessCheckFilter.SuccessOnly;
                        else if (((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.FilterFailedAccess) == (int)RegulationSettings.RegulationServerCategory.FilterFailedAccess) || (tempServer.AuditAccessCheck == AccessCheckFilter.FailureOnly))
                            tempServer.AuditAccessCheck = AccessCheckFilter.FailureOnly;
                        else
                            tempServer.AuditAccessCheck = AccessCheckFilter.NoFilter;
                    }
                }

                // apply HIPAA
                // the OR against the server settings is done because the user can select more than one template.  When more than one template is 
                // selected, the options are combined together
                if (checkHIPAA.Checked)
                {
                    if (_regulationSettings.TryGetValue((int)Regulation.RegulationType.HIPAA, out settings))
                    {
                        tempServer.AuditLogins = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.Logins) == (int)RegulationSettings.RegulationServerCategory.Logins) || tempServer.AuditLogins;
                        tempServer.AuditLogouts = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.Logouts) == (int)RegulationSettings.RegulationServerCategory.Logouts) || tempServer.AuditLogouts;
                        tempServer.AuditFailedLogins = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.FailedLogins) == (int)RegulationSettings.RegulationServerCategory.FailedLogins) || tempServer.AuditFailedLogins;
                        tempServer.AuditDDL = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.DatabaseDefinition) == (int)RegulationSettings.RegulationServerCategory.DatabaseDefinition) || tempServer.AuditDDL;
                        tempServer.AuditAdmin = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.AdminActivity) == (int)RegulationSettings.RegulationServerCategory.AdminActivity) || tempServer.AuditAdmin;
                        tempServer.AuditSecurity = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.SecurityChanges) == (int)RegulationSettings.RegulationServerCategory.SecurityChanges) || tempServer.AuditSecurity;
                        tempServer.AuditUDE = false;

                        if (((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.FilterPassedAccess) == (int)RegulationSettings.RegulationServerCategory.FilterPassedAccess) || (tempServer.AuditAccessCheck == AccessCheckFilter.SuccessOnly))
                            tempServer.AuditAccessCheck = AccessCheckFilter.SuccessOnly;
                        else if (((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.FilterFailedAccess) == (int)RegulationSettings.RegulationServerCategory.FilterFailedAccess) || (tempServer.AuditAccessCheck == AccessCheckFilter.FailureOnly))
                            tempServer.AuditAccessCheck = AccessCheckFilter.FailureOnly;
                        else
                            tempServer.AuditAccessCheck = AccessCheckFilter.NoFilter;
                    }
                }

                if (checkDISA.Checked)
                {
                    if (_regulationSettings.TryGetValue((int)Regulation.RegulationType.DISA, out settings))
                    {
                        tempServer.AuditLogins = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.Logins) == (int)RegulationSettings.RegulationServerCategory.Logins) || tempServer.AuditLogins;
                        tempServer.AuditLogouts = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.Logouts) == (int)RegulationSettings.RegulationServerCategory.Logouts) || tempServer.AuditLogouts;
                        tempServer.AuditFailedLogins = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.FailedLogins) == (int)RegulationSettings.RegulationServerCategory.FailedLogins) || tempServer.AuditFailedLogins;
                        tempServer.AuditDDL = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.DatabaseDefinition) == (int)RegulationSettings.RegulationServerCategory.DatabaseDefinition) || tempServer.AuditDDL;
                        tempServer.AuditAdmin = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.AdminActivity) == (int)RegulationSettings.RegulationServerCategory.AdminActivity) || tempServer.AuditAdmin;
                        tempServer.AuditSecurity = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.SecurityChanges) == (int)RegulationSettings.RegulationServerCategory.SecurityChanges) || tempServer.AuditSecurity;
                        tempServer.AuditUDE = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.UserDefined) == (int)RegulationSettings.RegulationServerCategory.UserDefined) || tempServer.AuditSecurity;
                        if (((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.FilterPassedAccess) == (int)RegulationSettings.RegulationServerCategory.FilterPassedAccess) || (tempServer.AuditAccessCheck == AccessCheckFilter.SuccessOnly))
                            tempServer.AuditAccessCheck = AccessCheckFilter.SuccessOnly;
                        else if (((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.FilterFailedAccess) == (int)RegulationSettings.RegulationServerCategory.FilterFailedAccess) || (tempServer.AuditAccessCheck == AccessCheckFilter.FailureOnly))
                            tempServer.AuditAccessCheck = AccessCheckFilter.FailureOnly;
                        else
                            tempServer.AuditAccessCheck = AccessCheckFilter.NoFilter;
                    }
                }
                if (checkNERC.Checked)
                {
                    if (_regulationSettings.TryGetValue((int)Regulation.RegulationType.NERC, out settings))
                    {
                        tempServer.AuditLogins = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.Logins) == (int)RegulationSettings.RegulationServerCategory.Logins) || tempServer.AuditLogins;
                        tempServer.AuditLogouts = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.Logouts) == (int)RegulationSettings.RegulationServerCategory.Logouts) || tempServer.AuditLogouts;
                        tempServer.AuditFailedLogins = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.FailedLogins) == (int)RegulationSettings.RegulationServerCategory.FailedLogins) || tempServer.AuditFailedLogins;
                        tempServer.AuditDDL = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.DatabaseDefinition) == (int)RegulationSettings.RegulationServerCategory.DatabaseDefinition) || tempServer.AuditDDL;
                        tempServer.AuditAdmin = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.AdminActivity) == (int)RegulationSettings.RegulationServerCategory.AdminActivity) || tempServer.AuditAdmin;
                        tempServer.AuditSecurity = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.SecurityChanges) == (int)RegulationSettings.RegulationServerCategory.SecurityChanges) || tempServer.AuditSecurity;
                        tempServer.AuditUDE = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.UserDefined) == (int)RegulationSettings.RegulationServerCategory.UserDefined) || tempServer.AuditSecurity;
                        if (((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.FilterPassedAccess) == (int)RegulationSettings.RegulationServerCategory.FilterPassedAccess) || (tempServer.AuditAccessCheck == AccessCheckFilter.SuccessOnly))
                            tempServer.AuditAccessCheck = AccessCheckFilter.SuccessOnly;
                        else if (((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.FilterFailedAccess) == (int)RegulationSettings.RegulationServerCategory.FilterFailedAccess) || (tempServer.AuditAccessCheck == AccessCheckFilter.FailureOnly))
                            tempServer.AuditAccessCheck = AccessCheckFilter.FailureOnly;
                        else
                            tempServer.AuditAccessCheck = AccessCheckFilter.NoFilter;
                    }
                }
                if (checkCIS.Checked)
                {
                    if (_regulationSettings.TryGetValue((int)Regulation.RegulationType.CIS, out settings))
                    {
                        tempServer.AuditLogins = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.Logins) == (int)RegulationSettings.RegulationServerCategory.Logins) || tempServer.AuditLogins;
                        tempServer.AuditLogouts = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.Logouts) == (int)RegulationSettings.RegulationServerCategory.Logouts) || tempServer.AuditLogouts;
                        tempServer.AuditFailedLogins = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.FailedLogins) == (int)RegulationSettings.RegulationServerCategory.FailedLogins) || tempServer.AuditFailedLogins;
                        tempServer.AuditDDL = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.DatabaseDefinition) == (int)RegulationSettings.RegulationServerCategory.DatabaseDefinition) || tempServer.AuditDDL;
                        tempServer.AuditAdmin = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.AdminActivity) == (int)RegulationSettings.RegulationServerCategory.AdminActivity) || tempServer.AuditAdmin;
                        tempServer.AuditSecurity = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.SecurityChanges) == (int)RegulationSettings.RegulationServerCategory.SecurityChanges) || tempServer.AuditSecurity;
                        tempServer.AuditUDE = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.UserDefined) == (int)RegulationSettings.RegulationServerCategory.UserDefined) || tempServer.AuditSecurity;
                        if (((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.FilterPassedAccess) == (int)RegulationSettings.RegulationServerCategory.FilterPassedAccess) || (tempServer.AuditAccessCheck == AccessCheckFilter.SuccessOnly))
                            tempServer.AuditAccessCheck = AccessCheckFilter.SuccessOnly;
                        else if (((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.FilterFailedAccess) == (int)RegulationSettings.RegulationServerCategory.FilterFailedAccess) || (tempServer.AuditAccessCheck == AccessCheckFilter.FailureOnly))
                            tempServer.AuditAccessCheck = AccessCheckFilter.FailureOnly;
                        else
                            tempServer.AuditAccessCheck = AccessCheckFilter.NoFilter;
                    }
                }
                if (checkSOX.Checked)
                {
                    if (_regulationSettings.TryGetValue((int)Regulation.RegulationType.SOX, out settings))
                    {
                        tempServer.AuditLogins = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.Logins) == (int)RegulationSettings.RegulationServerCategory.Logins) || tempServer.AuditLogins;
                        tempServer.AuditLogouts = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.Logouts) == (int)RegulationSettings.RegulationServerCategory.Logouts) || tempServer.AuditLogouts;
                        tempServer.AuditFailedLogins = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.FailedLogins) == (int)RegulationSettings.RegulationServerCategory.FailedLogins) || tempServer.AuditFailedLogins;
                        tempServer.AuditDDL = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.DatabaseDefinition) == (int)RegulationSettings.RegulationServerCategory.DatabaseDefinition) || tempServer.AuditDDL;
                        tempServer.AuditAdmin = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.AdminActivity) == (int)RegulationSettings.RegulationServerCategory.AdminActivity) || tempServer.AuditAdmin;
                        tempServer.AuditSecurity = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.SecurityChanges) == (int)RegulationSettings.RegulationServerCategory.SecurityChanges) || tempServer.AuditSecurity;
                        tempServer.AuditUDE = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.UserDefined) == (int)RegulationSettings.RegulationServerCategory.UserDefined) || tempServer.AuditSecurity;
                        if (((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.FilterPassedAccess) == (int)RegulationSettings.RegulationServerCategory.FilterPassedAccess) || (tempServer.AuditAccessCheck == AccessCheckFilter.SuccessOnly))
                            tempServer.AuditAccessCheck = AccessCheckFilter.SuccessOnly;
                        else if (((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.FilterFailedAccess) == (int)RegulationSettings.RegulationServerCategory.FilterFailedAccess) || (tempServer.AuditAccessCheck == AccessCheckFilter.FailureOnly))
                            tempServer.AuditAccessCheck = AccessCheckFilter.FailureOnly;
                        else
                            tempServer.AuditAccessCheck = AccessCheckFilter.NoFilter;
                    }
                }
                if (checkFERPA.Checked)
                {
                    if (_regulationSettings.TryGetValue((int)Regulation.RegulationType.FERPA, out settings))
                    {
                        tempServer.AuditLogins = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.Logins) == (int)RegulationSettings.RegulationServerCategory.Logins) || tempServer.AuditLogins;
                        tempServer.AuditLogouts = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.Logouts) == (int)RegulationSettings.RegulationServerCategory.Logouts) || tempServer.AuditLogouts;
                        tempServer.AuditFailedLogins = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.FailedLogins) == (int)RegulationSettings.RegulationServerCategory.FailedLogins) || tempServer.AuditFailedLogins;
                        tempServer.AuditDDL = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.DatabaseDefinition) == (int)RegulationSettings.RegulationServerCategory.DatabaseDefinition) || tempServer.AuditDDL;
                        tempServer.AuditAdmin = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.AdminActivity) == (int)RegulationSettings.RegulationServerCategory.AdminActivity) || tempServer.AuditAdmin;
                        tempServer.AuditSecurity = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.SecurityChanges) == (int)RegulationSettings.RegulationServerCategory.SecurityChanges) || tempServer.AuditSecurity;
                        tempServer.AuditUDE = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.UserDefined) == (int)RegulationSettings.RegulationServerCategory.UserDefined) || tempServer.AuditSecurity;
                        if (((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.FilterPassedAccess) == (int)RegulationSettings.RegulationServerCategory.FilterPassedAccess) || (tempServer.AuditAccessCheck == AccessCheckFilter.SuccessOnly))
                            tempServer.AuditAccessCheck = AccessCheckFilter.SuccessOnly;
                        else if (((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.FilterFailedAccess) == (int)RegulationSettings.RegulationServerCategory.FilterFailedAccess) || (tempServer.AuditAccessCheck == AccessCheckFilter.FailureOnly))
                            tempServer.AuditAccessCheck = AccessCheckFilter.FailureOnly;
                        else
                            tempServer.AuditAccessCheck = AccessCheckFilter.NoFilter;
                    }
                }
                if (checkGDPR.Checked)
                {
                    if (_regulationSettings.TryGetValue((int)Regulation.RegulationType.GDPR, out settings))
                    {
                        tempServer.AuditLogins = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.Logins) == (int)RegulationSettings.RegulationServerCategory.Logins) || tempServer.AuditLogins;
                        tempServer.AuditLogouts = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.Logouts) == (int)RegulationSettings.RegulationServerCategory.Logouts) || tempServer.AuditLogouts;
                        tempServer.AuditFailedLogins = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.FailedLogins) == (int)RegulationSettings.RegulationServerCategory.FailedLogins) || tempServer.AuditFailedLogins;
                        tempServer.AuditDDL = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.DatabaseDefinition) == (int)RegulationSettings.RegulationServerCategory.DatabaseDefinition) || tempServer.AuditDDL;
                        tempServer.AuditAdmin = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.AdminActivity) == (int)RegulationSettings.RegulationServerCategory.AdminActivity) || tempServer.AuditAdmin;
                        tempServer.AuditSecurity = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.SecurityChanges) == (int)RegulationSettings.RegulationServerCategory.SecurityChanges) || tempServer.AuditSecurity;
                        tempServer.AuditUDE = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.UserDefined) == (int)RegulationSettings.RegulationServerCategory.UserDefined) || tempServer.AuditSecurity;
                        if (((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.FilterPassedAccess) == (int)RegulationSettings.RegulationServerCategory.FilterPassedAccess) || (tempServer.AuditAccessCheck == AccessCheckFilter.SuccessOnly))
                            tempServer.AuditAccessCheck = AccessCheckFilter.SuccessOnly;
                        else if (((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.FilterFailedAccess) == (int)RegulationSettings.RegulationServerCategory.FilterFailedAccess) || (tempServer.AuditAccessCheck == AccessCheckFilter.FailureOnly))
                            tempServer.AuditAccessCheck = AccessCheckFilter.FailureOnly;
                        else
                            tempServer.AuditAccessCheck = AccessCheckFilter.NoFilter;
                    }
                }
            }
            //return the settings
            //SQLCM-5.6 SQLCM-5586 if conditions added so that regulatory guidelines be in additive in nature
            if (!srv.AuditLogins)
                srv.AuditLogins = tempServer.AuditLogins;

            if (!srv.AuditLogouts)
                srv.AuditLogouts = tempServer.AuditLogouts;

            if (!srv.AuditFailedLogins)
                srv.AuditFailedLogins = tempServer.AuditFailedLogins;

            if (!srv.AuditDDL)
                srv.AuditDDL = tempServer.AuditDDL;

            if (!srv.AuditAdmin)
                srv.AuditAdmin = tempServer.AuditAdmin;

            if (!srv.AuditSecurity)
                srv.AuditSecurity = tempServer.AuditSecurity;

            if (!srv.AuditUDE)
                srv.AuditUDE = tempServer.AuditUDE;


            srv.AuditAccessCheck = tempServer.AuditAccessCheck;

            RegulationSettings.UpdateRegulationSettingsServer(Globals.Repository.Connection, srv.SrvId, updatedSettings); //Update user applied regulation settings
        }

        private bool ValidatePrivilegedUsers()
        {
            if (lstPrivilegedUsers.Items.Count > 0 &&
               rbAuditUserSelected.Checked)
            {
                // make sure something checked
                if (!chkUserAuditLogins.Checked &&
                   !chkUserAuditFailedLogins.Checked &&
                   !chkUserAuditSecurity.Checked &&
                   !chkUserAuditAdmin.Checked &&
                   !chkUserAuditDDL.Checked &&
                   !chkUserAuditDML.Checked &&
                   !chkUserAuditSELECT.Checked &&
                   !chkUserAuditUDE.Checked)
                {
                    ErrorMessage.Show(this.Text, UIConstants.Error_MustSelectOneAuditUserOption);
                    _currentPage = WizardPage.PrivilegedUserEvents;
                    ShowPage(WizardDirection.Previous);
                    return false;
                }
            }
            return true;
        }

        private bool SaveDatabaseRecords()
        {
            bool retVal = true;
            bool isDirty = false;
            string errorMsg = String.Empty;
            SqlTransaction transaction;

            // If this was from a click on the "Apply Regulation" we just need to update the existing dbs.
            if (_startPage == StartPage.RegulationGuidelineInfo)
            {
                bool ret = UpdateDatabases();
                return ret;
            }
            else
            {
                // otherwise, add the newly selected ones.
                for (int ndx = 0; ndx < listDatabases.CheckedItems.Count; ndx++)
                {
                    RawDatabaseObject rawDB = (RawDatabaseObject)listDatabases.CheckedItems[ndx];
                    DatabaseRecord db = CreateDatabaseRecord(rawDB.name, rawDB.dbid);

                    try
                    {
                        using (TransactionScope transactionScope = new TransactionScope())
                        {
                            //start sqlcm 5.6 - 5467
                            if (_auditType == AuditingType.Default)
                            {
                                ApplyDefaultDatabaseSettings(db);
                            }
                            //end sqlcm 5.6 - 5467

                            //start sqlcm 5.6 - 5464(adding database level privileged role while applying server level guidelines)
                            if (Globals.isServerNodeSelected && lstPrivilegedUsersdb.Items.Count > 0)
                            {
                                UserList userList = GetPrivilegedDatabaseLevelUserProperty();

                                if (userList != null && _chkPriviligedUser.Checked)
                                {
                                    db.AuditPrivUsersList = userList.ToString();
                                }
                            }
                            else
                            {
                                if (_auditType != AuditingType.Default)
                                    db.AuditPrivUsersList = GetPrivilegedUserProperty();
                            }

                            //end sqlcm 5.6 - 5464
                            if (!WriteDatabaseRecord(db, null))
                            {
                                retVal = false;
                                // TODO: ask user if they want to contine
                            }
                            else
                            {
                                isDirty = true;
                                if (db.AuditDataChanges)
                                {
                                    List<DataChangeTableRecord> badRecords = GetBATables(db.Name);
                                    if (badRecords.Count == 0)
                                    {
                                        DataChangeTableRecord.DeleteUserTables(Globals.Repository.Connection, _server.SrvId, db.DbId, null);
                                    }
                                    else
                                    {
                                        DataChangeTableRecord.CreateUserTables(Globals.Repository.Connection, badRecords, _server.SrvId, db.DbId, null);
                                    }
                                }
                                if (db.AuditSensitiveColumns)
                                {
                                    List<SensitiveColumnTableRecord> scRecords = GetSCTables(db.Name, db.DbId, null);

                                    //If there are no tables, clear it out.  The point of applying a regulation is to override the current settings.
                                    if (scRecords.Count == 0)
                                        retVal = SensitiveColumnTableRecord.DeleteUserTables(Globals.Repository.Connection, _server.SrvId, db.DbId, null);
                                    else
                                        //add the new tables (this will delete all the old tables from this DB)
                                        retVal = SensitiveColumnTableRecord.CreateUserTables(Globals.Repository.Connection, scRecords, _server.SrvId, db.DbId, null);
                                }
                                else
                                    retVal = true;
                            }

                            transactionScope.Complete();
                        }
                    }
                    catch (Exception ex)
                    {
                        errorMsg = ex.Message;
                    }
                    finally
                    {
                        if (retVal && isDirty)
                        {
                            // log addition
                            ServerUpdate.RegisterChange(db.SrvId,
                                                        LogType.NewDatabase,
                                                        db.SrvInstance,
                                                        Snapshot.DatabaseSnapshot(Globals.Repository.Connection, db.DbId, db.Name, true));

                            // remove from list in case there is a error on some other db
                            listDatabases.Items.Remove(listDatabases.CheckedItems[ndx]);
                            // because we just decremented the count and changed the list!
                            ndx--;
                            _databases.Add(db);
                        }

                        if (!retVal)
                        {
                            ErrorMessage.Show(this.Text, UIConstants.Error_ErrorSavingDatabase, errorMsg);
                        }
                    }
                    //SQLCM 5.7 Requirement 5.3.4.2
                    IFormatter formatter;
                    MemoryStream streamTrustedUsers = null;
                    MemoryStream streamPrivilegedUsers = null;
                    streamTrustedUsers = new MemoryStream(Convert.FromBase64String(db.AuditUsersList));
                    streamPrivilegedUsers = new MemoryStream(Convert.FromBase64String(db.AuditPrivUsersList));
                    formatter = new BinaryFormatter();
                    RemoteUserList trustedUserList = new RemoteUserList();
                    RemoteUserList privilegedUserList = new RemoteUserList();
                    if (db.AuditUsersList != null && db.AuditUsersList != "")
                    {
                        trustedUserList = (RemoteUserList)formatter.Deserialize(streamTrustedUsers);
                    }
                    if (db.AuditPrivUsersList != null && db.AuditPrivUsersList != "")
                    {
                        privilegedUserList = (RemoteUserList)formatter.Deserialize(streamPrivilegedUsers);
                    }
                    var error = string.Empty;
                    try
                    {
                        if (!db.SaveDatabaseLevelUsersFromServerSettings(db, trustedUserList, privilegedUserList))
                        {
                            error = DatabaseRecord.GetLastError();
                            throw (new Exception(error));
                        }
                        else
                            retVal = true;
                    }
                    catch (Exception ex)
                    {
                        error = ex.Message;
                    }
                }//for
            }

            // copy the primary dbs for a Secondary node
            // Create a function here that will copy the Primary dbs in the table, 
            // Here we will have 'No'/'Read-intent only' and 'replica server name which is secondary'             
            foreach (SecondaryRoleDetails db in _secondaryRoleDetailsList)
            {
                try
                {
                    if (
                        ((db.secondaryRoleAllowConnections == 0) || (db.secondaryRoleAllowConnections == 1))
                        &&
                        (db.replicaServerName == _server.Instance)
                        &&
                        (!db.isPrimary)
                       )
                    {
                        // Check if the same group contains a Primary server in the Database table
                        // If yes, copy the Primary db details                         
                        bool retval = AddSecondaryDatabases(db.isPrimary, _server.SrvId, _server.Instance, db.avgName, Globals.Repository.Connection);

                        // Create a db record of the inserted secondary dbs
                        List<DatabaseRecord> secondaryDbRecord = GetSecondaryDatabaseRecord(_server.SrvId, Globals.Repository.Connection);
                        if (retval)
                        {
                            foreach (DatabaseRecord secondaryDb in secondaryDbRecord)
                            {
                                _databases.Add(secondaryDb);
                            }
                        }
                    }

                }

                catch (Exception ex)
                {
                    ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                        String.Format("AddSecondaryDatabases: Instance {0}", _server.Instance), ex, ErrorLog.Severity.Error);
                }

            }
            return retVal;
        }

        private bool UpdateDatabases()
        {
            SqlTransaction transaction;
            DatabaseRecord newDb;
            bool retVal = false;
            bool isDirty = false;
            string errorMsg = String.Empty;

            foreach (DatabaseRecord oldDb in _databases)
            {
                newDb = oldDb.Clone();
                newDb.Connection = Globals.Repository.Connection;
                ApplyDatabaseRegulation(newDb);
                if (!Globals.isServerNodeSelected)
                {
                    // Database Privileged User settings
                    newDb.AuditPrivUsersList = GetPrivilegedUserProperty();
                    newDb.AuditUserAll = rbAuditUserAll.Checked;
                    newDb.AuditUserLogins = chkUserAuditLogins.Checked;
                    newDb.AuditUserLogouts = chkUserAuditLogouts.Checked;
                    newDb.AuditUserFailedLogins = chkUserAuditFailedLogins.Checked;
                    newDb.AuditUserDDL = chkUserAuditDDL.Checked;
                    newDb.AuditUserSecurity = chkUserAuditSecurity.Checked;
                    newDb.AuditUserAdmin = chkUserAuditAdmin.Checked;
                    newDb.AuditUserDML = chkUserAuditDML.Checked;
                    newDb.AuditUserSELECT = chkUserAuditSELECT.Checked;

                    newDb.AuditUserUDE = chkUserAuditUDE.Checked;
                    if (chkUserAccessCheckFilter.Checked)
                    {
                        if (rbUserAuditFailedOnly.Checked)
                            newDb.AuditUserAccessCheck = AccessCheckFilter.FailureOnly;
                        else
                            newDb.AuditUserAccessCheck = AccessCheckFilter.SuccessOnly;
                    }
                    else
                    {
                        newDb.AuditUserAccessCheck = AccessCheckFilter.NoFilter;
                    }
                    newDb.AuditUserCaptureSQL = chkUserAuditCaptureSQL.Enabled && chkUserAuditCaptureSQL.Checked;
                    newDb.AuditUserCaptureTrans = chkUserAuditCaptureTrans.Enabled && chkUserAuditCaptureTrans.Checked;
                    newDb.AuditUserCaptureDDL = chkUserCaptureDDL.Enabled && chkUserCaptureDDL.Checked;
                }
                else
                {
                    //start sqlcm 5.6 - 5464(adding database level privileged role while applying server level guidelines)
                    UserList userList = GetPrivilegedDatabaseLevelUserProperty();
                    if (userList != null && _chkPriviligedUser.Checked)
                    {
                        UserList oldUserList = new UserList(oldDb.AuditPrivUsersList);
                        foreach (Login l in userList.Logins)
                        {
                            if (!oldUserList.Logins.Contains(l))
                                oldUserList.AddLogin(l);
                        }
                        foreach (ServerRole r in userList.ServerRoles)
                        {
                            if (!oldUserList.ServerRoles.Contains(r))
                                oldUserList.AddServerRole(r);
                        }
                        newDb.AuditPrivUsersList = oldUserList.ToString();
                    }

                    newDb.AuditUserAll = rbAuditUserAllDatabase.Checked;
                    newDb.AuditUserLogins = chkUserAuditLoginsDatabase.Checked;
                    newDb.AuditUserLogouts = chkUserAuditLogoutsDatabase.Checked;
                    newDb.AuditUserFailedLogins = chkUserAuditFailedLoginsDatabase.Checked;
                    newDb.AuditUserDDL = chkUserAuditDDLDatabase.Checked;
                    newDb.AuditUserSecurity = chkUserAuditSecurityDatabase.Checked;
                    newDb.AuditUserAdmin = chkUserAuditAdminDatabase.Checked;
                    newDb.AuditUserDML = chkUserAuditDMLDatabase.Checked;
                    newDb.AuditUserSELECT = chkUserAuditSELECTDatabase.Checked;

                    newDb.AuditUserUDE = chkUserAuditUDEDatabase.Checked;
                    if (chkUserAccessCheckFilterDatabase.Checked)
                    {
                        if (rbUserAuditFailedOnlyDatabase.Checked)
                            newDb.AuditUserAccessCheck = AccessCheckFilter.FailureOnly;
                        else
                            newDb.AuditUserAccessCheck = AccessCheckFilter.SuccessOnly;
                    }
                    else
                    {
                        newDb.AuditUserAccessCheck = AccessCheckFilter.NoFilter;
                    }
                    newDb.AuditUserCaptureSQL = chkUserAuditCaptureSQLDatabase.Enabled && chkUserAuditCaptureSQLDatabase.Checked;
                    newDb.AuditUserCaptureTrans = chkUserAuditCaptureTransDatabase.Enabled && chkUserAuditCaptureTransDatabase.Checked;
                    newDb.AuditUserCaptureDDL = chkUserCaptureDDLDatabase.Enabled && chkUserCaptureDDLDatabase.Checked;

                    //end sqlcm 5.6 - 5464
                }
                using (transaction = Globals.Repository.Connection.BeginTransaction())
                {
                    try
                    {
                        if (!newDb.Write(oldDb, transaction))
                        {
                            errorMsg = DatabaseRecord.GetLastError();
                        }
                        else
                        {
                            isDirty = true;

                            if (newDb.AuditDataChanges)
                            {
                                List<DataChangeTableRecord> badRecords = GetBATables(newDb.Name);
                                if (badRecords.Count == 0)
                                    //If there are no tables, clear it out.  The point of applying a regulation is to override the current settings.
                                    DataChangeTableRecord.DeleteUserTables(Globals.Repository.Connection, _server.SrvId, newDb.DbId, transaction);

                                else
                                    //add the new tables (this will delete all the old tables from this DB)
                                    DataChangeTableRecord.CreateUserTables(Globals.Repository.Connection, badRecords, _server.SrvId, newDb.DbId, transaction);
                            }
                            else
                                retVal = true;
                            if (newDb.AuditSensitiveColumns)
                            {
                                List<SensitiveColumnTableRecord> scRecords = GetSCTables(newDb.Name, newDb.DbId, transaction);

                                //If there are no tables, clear it out.  The point of applying a regulation is to override the current settings.
                                if (scRecords.Count == 0)
                                    retVal = SensitiveColumnTableRecord.DeleteUserTables(Globals.Repository.Connection, _server.SrvId, newDb.DbId, transaction);
                                else
                                    //add the new tables (this will delete all the old tables from this DB)
                                    retVal = SensitiveColumnTableRecord.CreateUserTables(Globals.Repository.Connection, scRecords, _server.SrvId, newDb.DbId, transaction);
                            }

                            else
                                retVal = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        errorMsg = ex.Message;
                    }
                    finally
                    {
                        if (transaction != null)
                        {
                            if (retVal && isDirty)
                            {
                                transaction.Commit();

                                string changeLog = Snapshot.DatabaseChangeLog(Globals.Repository.Connection,
                                                                            oldDb,
                                                                            newDb,
                                                                            Snapshot.GetDatabaseTables(Globals.Repository.Connection, oldDb.DbId, "\t\t"),
                                                                            Snapshot.GetDataChangeTables(Globals.Repository.Connection, oldDb.DbId, "\t\t"),
                                                                            Snapshot.GetSensitiveColumnTables(Globals.Repository.Connection, oldDb.DbId, "\t\t"));

                                // Register change to server and perform audit log				      
                                ServerUpdate.RegisterChange(newDb.SrvId, LogType.ModifyDatabase, newDb.SrvInstance, changeLog);
                            }
                            else
                            {
                                transaction.Rollback();
                            }
                        }

                        //SQLCM 5.7 Requirement 5.3.4.2
                        IFormatter formatter;
                        MemoryStream streamTrustedUsers = null;
                        MemoryStream streamPrivilegedUsers = null;
                        streamTrustedUsers = new MemoryStream(Convert.FromBase64String(newDb.AuditUsersList));
                        streamPrivilegedUsers = new MemoryStream(Convert.FromBase64String(newDb.AuditPrivUsersList));
                        formatter = new BinaryFormatter();
                        RemoteUserList trustedUserList = new RemoteUserList();
                        RemoteUserList privilegedUserList = new RemoteUserList();
                        if (newDb.AuditUsersList != null && newDb.AuditUsersList != "")
                        {
                            trustedUserList = (RemoteUserList)formatter.Deserialize(streamTrustedUsers);
                        }
                        if (newDb.AuditPrivUsersList != null && newDb.AuditPrivUsersList != "")
                        {
                            privilegedUserList = (RemoteUserList)formatter.Deserialize(streamPrivilegedUsers);
                        }
                        var error = string.Empty;
                        try
                        {
                            if (!newDb.SaveDatabaseLevelUsersFromServerSettings(newDb, trustedUserList, privilegedUserList))
                            {
                                error = DatabaseRecord.GetLastError();
                                throw (new Exception(error));
                            }
                            else
                                retVal = true;
                        }
                        catch (Exception ex)
                        {
                            error = ex.Message;
                        }
                        if (!retVal)
                        {
                            ErrorMessage.Show(this.Text, UIConstants.Error_ErrorSavingDatabase, errorMsg);
                        }
                    }
                }
            }
            return retVal;
        }

        private DatabaseRecord CreateDatabaseRecord(string dbName, int dbId)
        {
            DatabaseRecord db = new DatabaseRecord();
            db.Connection = Globals.Repository.Connection;

            // General
            db.SrvId = _server == null ? -1 : _server.SrvId;
            db.SrvInstance = _server.Instance;
            db.Name = dbName;
            db.SqlDatabaseId = dbId;
            db.Description = "";
            db.IsEnabled = true;
            db.IsSqlSecureDb = false;
            db.IsAlwaysOn = false;
            db.ReplicaServers = "";
            if (_dbAVGList != null)
            {
                foreach (RawAVGroup ravg in _dbAVGList)
                {
                    if (ravg.dbName.Equals(dbName))
                    {
                        if (db.ReplicaServers.Length == 0)
                        {
                            db.IsAlwaysOn = true;
                            db.AvailGroupName = ravg.avgName;
                            db.ReplicaServers = ravg.replicaServerName;
                            db.IsPrimary = ravg.isPrimary;
                        }
                        else
                            db.ReplicaServers += COMMA_CHARACTER + ravg.replicaServerName;
                    }
                }
            }
            // v5.6 SQLCM-5373
            if (_startPage == StartPage.Server)
                db.AuditUsersList = GetServerLevelTrustedUserProperty();//Trusted Uses at Server level
            else
                db.AuditUsersList = GetTrustedUserProperty(); //Trusted Uses at Database level

            //start sqlcm 5.6 - 5464
            if (Globals.isServerNodeSelected && _chkPriviligedUser.Checked && lstPrivilegedUsersdb.Items.Count > 0)
            {



                db.AuditUserAll = rbAuditUserAllDatabase.Checked;
                db.AuditUserLogins = chkUserAuditLoginsDatabase.Checked;
                db.AuditUserLogouts = chkUserAuditLogoutsDatabase.Checked;
                db.AuditUserFailedLogins = chkUserAuditFailedLoginsDatabase.Checked;
                db.AuditUserDDL = chkUserAuditDDLDatabase.Checked;
                db.AuditUserSecurity = chkUserAuditSecurityDatabase.Checked;
                db.AuditUserAdmin = chkUserAuditAdminDatabase.Checked;
                db.AuditUserDML = chkUserAuditDMLDatabase.Checked;
                db.AuditUserSELECT = chkUserAuditSELECTDatabase.Checked;
                db.AuditUserUDE = chkUserAuditUDEDatabase.Checked;
                if (chkUserAccessCheckFilterDatabase.Checked)
                {
                    if (rbUserAuditSuccessfulOnlyDatabase.Checked)
                        db.AuditUserAccessCheck = AccessCheckFilter.SuccessOnly;
                    else
                        db.AuditUserAccessCheck = AccessCheckFilter.FailureOnly;
                }
                else
                {
                    db.AuditUserAccessCheck = AccessCheckFilter.NoFilter;
                }
                db.AuditUserCaptureSQL = chkUserAuditCaptureSQLDatabase.Checked;
                db.AuditUserCaptureTrans = chkUserAuditCaptureTransDatabase.Checked;
                db.AuditUserCaptureDDL = chkUserCaptureDDLDatabase.Checked;
                db.AuditUserExceptions = false;

                //DML only setting
                if (rbAuditUserSelectedDatabase.Checked && chkUserAuditDMLDatabase.Checked)
                    chkUserAuditCaptureTransDatabase.Enabled = true;
                else
                    chkUserAuditCaptureTransDatabase.Enabled = false;

                //DML or SELECT setting
                if (rbAuditUserSelectedDatabase.Checked && (chkUserAuditDMLDatabase.Checked || chkUserAuditSELECTDatabase.Checked) && CoreConstants.AllowCaptureSql)
                    chkUserAuditCaptureSQLDatabase.Enabled = true;
                else
                {
                    chkUserAuditCaptureSQLDatabase.Checked = false;
                    chkUserAuditCaptureSQLDatabase.Enabled = false;
                }
            }
            else
            {


                db.AuditUserAll = rbAuditUserAll.Checked;
                db.AuditUserLogins = chkUserAuditLogins.Checked;
                db.AuditUserLogouts = chkUserAuditLogouts.Checked;
                db.AuditUserFailedLogins = chkUserAuditFailedLogins.Checked;
                db.AuditUserDDL = chkUserAuditDDL.Checked;
                db.AuditUserSecurity = chkUserAuditSecurity.Checked;
                db.AuditUserAdmin = chkUserAuditAdmin.Checked;
                db.AuditUserDML = chkUserAuditDML.Checked;
                db.AuditUserSELECT = chkUserAuditSELECT.Checked;
                db.AuditUserUDE = chkUserAuditUDE.Checked;
                if (chkUserAccessCheckFilter.Checked)
                {
                    if (rbUserAuditSuccessfulOnly.Checked)
                        db.AuditUserAccessCheck = AccessCheckFilter.SuccessOnly;
                    else
                        db.AuditUserAccessCheck = AccessCheckFilter.FailureOnly;
                }
                else
                {
                    db.AuditUserAccessCheck = AccessCheckFilter.NoFilter;
                }
                db.AuditUserCaptureSQL = chkUserAuditCaptureSQL.Checked;
                db.AuditUserCaptureTrans = chkUserAuditCaptureTrans.Checked;
                db.AuditUserCaptureDDL = chkUserCaptureDDL.Checked;
                db.AuditUserExceptions = false;

                //DML only setting
                if (rbAuditUserSelected.Checked && chkUserAuditDML.Checked)
                    chkUserAuditCaptureTrans.Enabled = true;
                else
                    chkUserAuditCaptureTrans.Enabled = false;

                //DML or SELECT setting
                if (rbAuditUserSelected.Checked && (chkUserAuditDML.Checked || chkUserAuditSELECT.Checked) && CoreConstants.AllowCaptureSql)
                    chkUserAuditCaptureSQL.Enabled = true;
                else
                {
                    chkUserAuditCaptureSQL.Checked = false;
                    chkUserAuditCaptureSQL.Enabled = false;
                }
            }
            //end sqlcm 5.6 -5464

            if (radioDefault.Checked)
            {
                // Audit Settings		
                db.AuditDDL = true;
                db.AuditSecurity = true;
                db.AuditAdmin = true;
                db.AuditDML = false;
                db.AuditSELECT = false;
                db.AuditAccessCheck = AccessCheckFilter.SuccessOnly;
                db.AuditCaptureSQL = false;
                db.AuditCaptureTrans = false;
                db.AuditExceptions = false;
                db.AuditDmlAll = true;
                db.AuditUserTables = Globals.SQLcomplianceConfig.AuditUserTables;
                db.AuditSystemTables = Globals.SQLcomplianceConfig.AuditSystemTables;
                db.AuditStoredProcedures = Globals.SQLcomplianceConfig.AuditStoredProcedures;
                db.AuditDmlOther = Globals.SQLcomplianceConfig.AuditDmlOther;
            }
            else
            {
                // Audit Settings		
                if (_auditType == AuditingType.Regulation)
                {
                    ApplyDatabaseRegulation(db);
                }
                else
                {
                    db.AuditDDL = chkDBAuditDDL.Checked;
                    db.AuditSecurity = chkDBAuditSecurity.Checked;
                    db.AuditAdmin = chkDBAuditAdmin.Checked;
                    db.AuditDML = chkDBAuditDML.Checked;
                    db.AuditSELECT = chkDBAuditSELECT.Checked;
                    if (chkDBAccessCheckFilter.Checked)
                    {
                        if (rbDBAuditFailedOnly.Checked)
                            db.AuditAccessCheck = AccessCheckFilter.FailureOnly;
                        else
                            db.AuditAccessCheck = AccessCheckFilter.SuccessOnly;
                    }
                    else
                    {
                        db.AuditAccessCheck = AccessCheckFilter.NoFilter;
                    }
                    db.AuditCaptureSQL = CoreConstants.AllowCaptureSql && chkDBAuditCaptureSQL.Checked;
                    db.AuditCaptureDDL = CoreConstants.AllowCaptureSql && chkDBCaptureDDL.Checked;
                    db.AuditCaptureTrans = chkDBAuditCaptureTrans.Checked;
                }
                db.AuditExceptions = false;
                if (db.AuditDML || db.AuditSELECT)
                {
                    if (radioAllDML.Checked)
                    {
                        db.AuditDmlAll = true;

                        // set other values to glabl defaults		         
                        db.AuditUserTables = Globals.SQLcomplianceConfig.AuditUserTables;
                        db.AuditSystemTables = Globals.SQLcomplianceConfig.AuditSystemTables;
                        db.AuditStoredProcedures = Globals.SQLcomplianceConfig.AuditStoredProcedures;
                        db.AuditDmlOther = Globals.SQLcomplianceConfig.AuditDmlOther;
                    }
                    else
                    {
                        db.AuditDmlAll = false;

                        // User Tables (not boolean; 2 means select specific tables; done in DB properties)
                        if (chkAuditUserTables.Checked)
                            db.AuditUserTables = 1;
                        else
                            db.AuditUserTables = 0;

                        // Audit Objects
                        db.AuditSystemTables = chkAuditSystemTables.Checked;
                        db.AuditStoredProcedures = chkAuditStoredProcedures.Checked;
                        db.AuditDmlOther = chkAuditOther.Checked;
                    }
                }
                else
                {
                    // save global defaults
                    db.AuditDmlAll = Globals.SQLcomplianceConfig.AuditDmlAll;
                    db.AuditUserTables = Globals.SQLcomplianceConfig.AuditUserTables;
                    db.AuditSystemTables = Globals.SQLcomplianceConfig.AuditSystemTables;
                    db.AuditStoredProcedures = Globals.SQLcomplianceConfig.AuditStoredProcedures;
                    db.AuditDmlOther = Globals.SQLcomplianceConfig.AuditDmlOther;
                }
            }
            return db;
        }
        //start sqlcm 5.6 - 5467
        public void ApplyDefaultDatabaseSettings(DatabaseRecord db)
        {
            try
            {
                var dbSettings = DefaultAuditSettingsHelper.GetDBAuditSettings(_instanceName);
                //this object can be assigned directly as in db object, there might be other propertise set so just assign the required ones

                db.AuditDDL = dbSettings.AuditDDL;
                db.AuditSecurity = dbSettings.AuditSecurity;
                db.AuditAdmin = dbSettings.AuditAdmin;
                db.AuditDML = dbSettings.AuditDML;
                db.AuditSELECT = dbSettings.AuditSELECT;
                db.AuditAccessCheck = dbSettings.AuditAccessCheck;
                db.AuditCaptureDDL = dbSettings.AuditCaptureDDL;
                db.AuditCaptureTrans = dbSettings.AuditCaptureTrans;
                db.AuditCaptureSQL = dbSettings.AuditCaptureSQL;
                db.AuditUsersList = dbSettings.AuditUsersList;
                db.AuditPrivUsersList = dbSettings.AuditPrivUsersList;
                db.AuditUserAll = dbSettings.AuditUserAll;
                db.AuditUserLogins = dbSettings.AuditUserLogins;
                db.AuditUserLogouts = dbSettings.AuditUserLogouts;
                db.AuditUserFailedLogins = dbSettings.AuditUserFailedLogins;
                db.AuditUserDDL = dbSettings.AuditUserDDL;
                db.AuditUserSecurity = dbSettings.AuditUserSecurity;
                db.AuditUserAdmin = dbSettings.AuditUserAdmin;
                db.AuditUserDML = dbSettings.AuditUserDML;
                db.AuditUserSELECT = dbSettings.AuditUserSELECT;
                db.AuditUserUDE = dbSettings.AuditUserUDE;
                db.AuditUserAccessCheck = dbSettings.AuditUserAccessCheck;
                db.AuditUserCaptureSQL = dbSettings.AuditUserCaptureSQL;
                db.AuditUserCaptureTrans = dbSettings.AuditUserCaptureTrans;
                db.AuditUserCaptureDDL = dbSettings.AuditUserCaptureDDL;

                Close();

            }
            catch (Exception ex)
            {
                MessageBox.Show("" + ex);
            }
        }
        //end sqlcm 5.6 - 5467
        private List<DatabaseRecord> GetSecondaryDatabaseRecord(int srvId, SqlConnection conn)
        {
            List<DatabaseRecord> secondaryDatabaseRecords = new List<DatabaseRecord>();
            try
            {
                DatabaseRecord db = new DatabaseRecord();
                secondaryDatabaseRecords = db.GetSecondaryDatabaseRecords(srvId, conn);
            }

            catch (Exception ex)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                         "GetSecondaryDatabaseRecords",
                                         ex);
            }

            return secondaryDatabaseRecords;
        }

        private List<SensitiveColumnTableRecord> GetSCTables(string dbName, int dbId, SqlTransaction transaction)
        {
            List<SensitiveColumnTableRecord> retVal = new List<SensitiveColumnTableRecord>();
            int? maxColId = null;
            string errorMsg = "";
            _dbName = dbName;
            ListView temp_lvSCTables = new ListView();

            //Get Max Column Id
            try
            {
                maxColId = SensitiveColumnTableRecord.GetMaxColId(Globals.Repository.Connection, transaction);
            }

            catch (Exception ex)
            {
                errorMsg = ex.Message;
            }
            if (_dictLvSCTables.TryGetValue(dbName, out temp_lvSCTables))
            {
                foreach (ListViewItem item in temp_lvSCTables.Items)
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
                                // sctItem.ColumnId = 0; // In case of Individual, we will always set columnId to 0
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

                                foreach (string col in Form_TableConfigure.GetColumns(item.SubItems[1].Text))
                                {
                                    sctItem.AddColumn(col);
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
                                builder.Append(sctItem.FullTableName + "." + o.ColumnName).Append(",");
                            }
                            builder.Remove(builder.Length - 1, 1);
                            columnNames = builder.ToString();

                            // Insert all columns into an Array
                            string[] allColumns = columnNames.Split(',');

                            //Get current selected columns and insert into an Array
                            string[] slectedColumns = item.SubItems[1].Text.Split(',');

                            // Get common column names
                            string[] result = allColumns.Intersect(slectedColumns).ToArray();

                            // Add the common columns to scItem
                            foreach (string col in result)
                            {
                                sctItem.AddColumn(col);
                            }

                            retVal.Add(sctItem);
                        }
                    }
                }
            }
            return retVal;
        }

        private void ApplyDatabaseRegulation(DatabaseRecord db)
        {
            RegulationSettings settings;
            DatabaseRecord tempDb = new DatabaseRecord();

            if (_isCustomTemplate || checkCustom.Checked)
            {
                tempDb.AuditDDL = _chkAuditDDL.Checked;
                tempDb.AuditSecurity = _chkAuditSecurityChanges.Checked;
                tempDb.AuditAdmin = _chkAuditAdminActivity.Checked;
                tempDb.AuditDML = _chkAuditDML.Checked;
                tempDb.AuditSELECT = _chkAuditSelect.Checked;
                tempDb.AuditCaptureSQL = _chkCaptureSQL.Checked;
                tempDb.AuditCaptureTrans = _chkCaptureTrans.Checked;
                tempDb.AuditCaptureDDL = CoreConstants.AllowCaptureSql && _chkDBCaptureDDL.Checked;
                tempDb.AuditSensitiveColumns = _chkAuditSensitiveColumn.Checked;
                tempDb.AuditDataChanges = _chkAuditBeforeAfter.Checked;

                if (_chkDBAccessCheckFilter.Checked)
                {
                    if (_rbDBAuditFailedOnly.Checked)
                        tempDb.AuditAccessCheck = AccessCheckFilter.FailureOnly;
                    else
                        tempDb.AuditAccessCheck = AccessCheckFilter.SuccessOnly;
                }
                else
                {
                    tempDb.AuditAccessCheck = AccessCheckFilter.NoFilter;
                }
                db.PCI = db.HIPAA = db.DISA = db.FERPA = db.CIS = db.SOX = db.NERC = db.GDPR = false;
            }
            else
            {

                // apply PCI
                if (checkPCI.Checked)
                {
                    db.PCI = true;

                    if (_regulationSettings.TryGetValue((int)Regulation.RegulationType.PCI, out settings))
                    {
                        tempDb.AuditDDL = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.DatabaseDefinition) == (int)RegulationSettings.RegulationDatabaseCategory.DatabaseDefinition) || tempDb.AuditDDL;
                        tempDb.AuditSecurity = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.SecurityChanges) == (int)RegulationSettings.RegulationDatabaseCategory.SecurityChanges) || tempDb.AuditSecurity;
                        tempDb.AuditAdmin = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.AdminActivity) == (int)RegulationSettings.RegulationDatabaseCategory.AdminActivity) || tempDb.AuditAdmin;
                        tempDb.AuditDML = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.DatabaseModification) == (int)RegulationSettings.RegulationDatabaseCategory.DatabaseModification) || tempDb.AuditDML;
                        tempDb.AuditSELECT = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.Select) == (int)RegulationSettings.RegulationDatabaseCategory.Select) || tempDb.AuditSELECT;

                        if ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.FilterPassedAccess) == (int)RegulationSettings.RegulationDatabaseCategory.FilterPassedAccess || (tempDb.AuditAccessCheck == AccessCheckFilter.SuccessOnly))
                            tempDb.AuditAccessCheck = AccessCheckFilter.SuccessOnly;
                        else if ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.FilterFailedAccess) == (int)RegulationSettings.RegulationDatabaseCategory.FilterFailedAccess || (tempDb.AuditAccessCheck == AccessCheckFilter.FailureOnly))
                            tempDb.AuditAccessCheck = AccessCheckFilter.FailureOnly;
                        else
                            tempDb.AuditAccessCheck = AccessCheckFilter.NoFilter;

                        tempDb.AuditCaptureSQL = CoreConstants.AllowCaptureSql && ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.SQLText) == (int)RegulationSettings.RegulationDatabaseCategory.SQLText) || tempDb.AuditCaptureSQL;
                        tempDb.AuditCaptureTrans = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.Transactions) == (int)RegulationSettings.RegulationDatabaseCategory.Transactions) || tempDb.AuditCaptureTrans;
                        tempDb.AuditSensitiveColumns = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.SensitiveColumns) == (int)RegulationSettings.RegulationDatabaseCategory.SensitiveColumns) || tempDb.AuditSensitiveColumns;
                        tempDb.AuditDataChanges = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.BeforeAfterDataChange) == (int)RegulationSettings.RegulationDatabaseCategory.BeforeAfterDataChange) || tempDb.AuditDataChanges;
                    }
                }
                else
                    db.PCI = false;

                // apply HIPAA
                // the OR against the server settings is done because the user can select more than one template.  When more than one template is 
                // selected, the options are combined togeher
                if (checkHIPAA.Checked)
                {
                    db.HIPAA = true;

                    if (_regulationSettings.TryGetValue((int)Regulation.RegulationType.HIPAA, out settings))
                    {
                        tempDb.AuditDDL = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.DatabaseDefinition) == (int)RegulationSettings.RegulationDatabaseCategory.DatabaseDefinition) || tempDb.AuditDDL;
                        tempDb.AuditSecurity = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.SecurityChanges) == (int)RegulationSettings.RegulationDatabaseCategory.SecurityChanges) || tempDb.AuditSecurity;
                        tempDb.AuditAdmin = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.AdminActivity) == (int)RegulationSettings.RegulationDatabaseCategory.AdminActivity) || tempDb.AuditAdmin;
                        tempDb.AuditDML = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.DatabaseModification) == (int)RegulationSettings.RegulationDatabaseCategory.DatabaseModification) || tempDb.AuditDML;
                        tempDb.AuditSELECT = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.Select) == (int)RegulationSettings.RegulationDatabaseCategory.Select) || tempDb.AuditSELECT;

                        if (((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.FilterPassedAccess) == (int)RegulationSettings.RegulationDatabaseCategory.FilterPassedAccess) || (tempDb.AuditAccessCheck == AccessCheckFilter.SuccessOnly))
                            tempDb.AuditAccessCheck = AccessCheckFilter.SuccessOnly;
                        else if (((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.FilterFailedAccess) == (int)RegulationSettings.RegulationDatabaseCategory.FilterFailedAccess) || (tempDb.AuditAccessCheck == AccessCheckFilter.FailureOnly))
                            tempDb.AuditAccessCheck = AccessCheckFilter.FailureOnly;
                        else
                            tempDb.AuditAccessCheck = AccessCheckFilter.NoFilter;

                        tempDb.AuditCaptureSQL = (CoreConstants.AllowCaptureSql && ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.SQLText) == (int)RegulationSettings.RegulationDatabaseCategory.SQLText)) || tempDb.AuditCaptureSQL;
                        tempDb.AuditCaptureTrans = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.Transactions) == (int)RegulationSettings.RegulationDatabaseCategory.Transactions) || tempDb.AuditCaptureTrans;
                        tempDb.AuditSensitiveColumns = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.SensitiveColumns) == (int)RegulationSettings.RegulationDatabaseCategory.SensitiveColumns) || tempDb.AuditSensitiveColumns;
                        tempDb.AuditDataChanges = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.BeforeAfterDataChange) == (int)RegulationSettings.RegulationDatabaseCategory.BeforeAfterDataChange) || tempDb.AuditDataChanges;
                    }
                }
                else
                    db.HIPAA = false;
                if (checkDISA.Checked)
                {
                    db.DISA = true;

                    if (_regulationSettings.TryGetValue((int)Regulation.RegulationType.DISA, out settings))
                    {
                        tempDb.AuditDDL = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.DatabaseDefinition) == (int)RegulationSettings.RegulationDatabaseCategory.DatabaseDefinition) || tempDb.AuditDDL;
                        tempDb.AuditSecurity = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.SecurityChanges) == (int)RegulationSettings.RegulationDatabaseCategory.SecurityChanges) || tempDb.AuditSecurity;
                        tempDb.AuditAdmin = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.AdminActivity) == (int)RegulationSettings.RegulationDatabaseCategory.AdminActivity) || tempDb.AuditAdmin;
                        tempDb.AuditDML = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.DatabaseModification) == (int)RegulationSettings.RegulationDatabaseCategory.DatabaseModification) || tempDb.AuditDML;
                        tempDb.AuditSELECT = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.Select) == (int)RegulationSettings.RegulationDatabaseCategory.Select) || tempDb.AuditSELECT;

                        if (((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.FilterPassedAccess) == (int)RegulationSettings.RegulationDatabaseCategory.FilterPassedAccess) || (tempDb.AuditAccessCheck == AccessCheckFilter.SuccessOnly))
                            tempDb.AuditAccessCheck = AccessCheckFilter.SuccessOnly;
                        else if (((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.FilterFailedAccess) == (int)RegulationSettings.RegulationDatabaseCategory.FilterFailedAccess) || (tempDb.AuditAccessCheck == AccessCheckFilter.FailureOnly))
                            tempDb.AuditAccessCheck = AccessCheckFilter.FailureOnly;
                        else
                            tempDb.AuditAccessCheck = AccessCheckFilter.NoFilter;

                        tempDb.AuditCaptureSQL = (CoreConstants.AllowCaptureSql && ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.SQLText) == (int)RegulationSettings.RegulationDatabaseCategory.SQLText)) || tempDb.AuditCaptureSQL;
                        tempDb.AuditCaptureTrans = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.Transactions) == (int)RegulationSettings.RegulationDatabaseCategory.Transactions) || tempDb.AuditCaptureTrans;
                        tempDb.AuditSensitiveColumns = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.SensitiveColumns) == (int)RegulationSettings.RegulationDatabaseCategory.SensitiveColumns) || tempDb.AuditSensitiveColumns;
                        tempDb.AuditDataChanges = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.BeforeAfterDataChange) == (int)RegulationSettings.RegulationDatabaseCategory.BeforeAfterDataChange) || tempDb.AuditDataChanges;
                    }
                }
                else
                    db.DISA = false;
                if (checkNERC.Checked)
                {
                    db.NERC = true;

                    if (_regulationSettings.TryGetValue((int)Regulation.RegulationType.NERC, out settings))
                    {
                        tempDb.AuditDDL = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.DatabaseDefinition) == (int)RegulationSettings.RegulationDatabaseCategory.DatabaseDefinition) || tempDb.AuditDDL;
                        tempDb.AuditSecurity = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.SecurityChanges) == (int)RegulationSettings.RegulationDatabaseCategory.SecurityChanges) || tempDb.AuditSecurity;
                        tempDb.AuditAdmin = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.AdminActivity) == (int)RegulationSettings.RegulationDatabaseCategory.AdminActivity) || tempDb.AuditAdmin;
                        tempDb.AuditDML = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.DatabaseModification) == (int)RegulationSettings.RegulationDatabaseCategory.DatabaseModification) || tempDb.AuditDML;
                        tempDb.AuditSELECT = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.Select) == (int)RegulationSettings.RegulationDatabaseCategory.Select) || tempDb.AuditSELECT;

                        if (((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.FilterPassedAccess) == (int)RegulationSettings.RegulationDatabaseCategory.FilterPassedAccess) || (tempDb.AuditAccessCheck == AccessCheckFilter.SuccessOnly))
                            tempDb.AuditAccessCheck = AccessCheckFilter.SuccessOnly;
                        else if (((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.FilterFailedAccess) == (int)RegulationSettings.RegulationDatabaseCategory.FilterFailedAccess) || (tempDb.AuditAccessCheck == AccessCheckFilter.FailureOnly))
                            tempDb.AuditAccessCheck = AccessCheckFilter.FailureOnly;
                        else
                            tempDb.AuditAccessCheck = AccessCheckFilter.NoFilter;

                        tempDb.AuditCaptureSQL = (CoreConstants.AllowCaptureSql && ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.SQLText) == (int)RegulationSettings.RegulationDatabaseCategory.SQLText)) || tempDb.AuditCaptureSQL;
                        tempDb.AuditCaptureTrans = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.Transactions) == (int)RegulationSettings.RegulationDatabaseCategory.Transactions) || tempDb.AuditCaptureTrans;
                        tempDb.AuditSensitiveColumns = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.SensitiveColumns) == (int)RegulationSettings.RegulationDatabaseCategory.SensitiveColumns) || tempDb.AuditSensitiveColumns;
                        tempDb.AuditDataChanges = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.BeforeAfterDataChange) == (int)RegulationSettings.RegulationDatabaseCategory.BeforeAfterDataChange) || tempDb.AuditDataChanges;
                    }
                }
                else
                    db.NERC = false;
                if (checkCIS.Checked)
                {
                    db.CIS = true;
                    if (!(checkPCI.Checked || checkHIPAA.Checked || checkDISA.Checked || checkNERC.Checked || checkSOX.Checked || checkFERPA.Checked || checkGDPR.Checked
                        || checkCustom.Checked))
                    {
                        tempDb.AuditDDL = true;
                        tempDb.AuditSecurity = true;
                        tempDb.AuditAdmin = true;
                        tempDb.AuditAccessCheck = AccessCheckFilter.SuccessOnly;
                    }
                }
                else
                    db.CIS = false;
                if (checkSOX.Checked)
                {
                    db.SOX = true;

                    if (_regulationSettings.TryGetValue((int)Regulation.RegulationType.SOX, out settings))
                    {
                        tempDb.AuditDDL = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.DatabaseDefinition) == (int)RegulationSettings.RegulationDatabaseCategory.DatabaseDefinition) || tempDb.AuditDDL;
                        tempDb.AuditSecurity = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.SecurityChanges) == (int)RegulationSettings.RegulationDatabaseCategory.SecurityChanges) || tempDb.AuditSecurity;
                        tempDb.AuditAdmin = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.AdminActivity) == (int)RegulationSettings.RegulationDatabaseCategory.AdminActivity) || tempDb.AuditAdmin;
                        tempDb.AuditDML = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.DatabaseModification) == (int)RegulationSettings.RegulationDatabaseCategory.DatabaseModification) || tempDb.AuditDML;
                        tempDb.AuditSELECT = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.Select) == (int)RegulationSettings.RegulationDatabaseCategory.Select) || tempDb.AuditSELECT;

                        if (((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.FilterPassedAccess) == (int)RegulationSettings.RegulationDatabaseCategory.FilterPassedAccess) || (tempDb.AuditAccessCheck == AccessCheckFilter.SuccessOnly))
                            tempDb.AuditAccessCheck = AccessCheckFilter.SuccessOnly;
                        else if (((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.FilterFailedAccess) == (int)RegulationSettings.RegulationDatabaseCategory.FilterFailedAccess) || (tempDb.AuditAccessCheck == AccessCheckFilter.FailureOnly))
                            tempDb.AuditAccessCheck = AccessCheckFilter.FailureOnly;
                        else
                            tempDb.AuditAccessCheck = AccessCheckFilter.NoFilter;

                        tempDb.AuditCaptureSQL = (CoreConstants.AllowCaptureSql && ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.SQLText) == (int)RegulationSettings.RegulationDatabaseCategory.SQLText)) || tempDb.AuditCaptureSQL;
                        tempDb.AuditCaptureTrans = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.Transactions) == (int)RegulationSettings.RegulationDatabaseCategory.Transactions) || tempDb.AuditCaptureTrans;
                        tempDb.AuditSensitiveColumns = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.SensitiveColumns) == (int)RegulationSettings.RegulationDatabaseCategory.SensitiveColumns) || tempDb.AuditSensitiveColumns;
                        tempDb.AuditDataChanges = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.BeforeAfterDataChange) == (int)RegulationSettings.RegulationDatabaseCategory.BeforeAfterDataChange) || tempDb.AuditDataChanges;
                    }
                }
                else
                    db.SOX = false;
                if (checkFERPA.Checked)
                {
                    db.FERPA = true;

                    if (_regulationSettings.TryGetValue((int)Regulation.RegulationType.FERPA, out settings))
                    {
                        tempDb.AuditDDL = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.DatabaseDefinition) == (int)RegulationSettings.RegulationDatabaseCategory.DatabaseDefinition) || tempDb.AuditDDL;
                        tempDb.AuditSecurity = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.SecurityChanges) == (int)RegulationSettings.RegulationDatabaseCategory.SecurityChanges) || tempDb.AuditSecurity;
                        tempDb.AuditAdmin = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.AdminActivity) == (int)RegulationSettings.RegulationDatabaseCategory.AdminActivity) || tempDb.AuditAdmin;
                        tempDb.AuditDML = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.DatabaseModification) == (int)RegulationSettings.RegulationDatabaseCategory.DatabaseModification) || tempDb.AuditDML;
                        tempDb.AuditSELECT = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.Select) == (int)RegulationSettings.RegulationDatabaseCategory.Select) || tempDb.AuditSELECT;

                        if (((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.FilterPassedAccess) == (int)RegulationSettings.RegulationDatabaseCategory.FilterPassedAccess) || (tempDb.AuditAccessCheck == AccessCheckFilter.SuccessOnly))
                            tempDb.AuditAccessCheck = AccessCheckFilter.SuccessOnly;
                        else if (((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.FilterFailedAccess) == (int)RegulationSettings.RegulationDatabaseCategory.FilterFailedAccess) || (tempDb.AuditAccessCheck == AccessCheckFilter.FailureOnly))
                            tempDb.AuditAccessCheck = AccessCheckFilter.FailureOnly;
                        else
                            tempDb.AuditAccessCheck = AccessCheckFilter.NoFilter;

                        tempDb.AuditCaptureSQL = (CoreConstants.AllowCaptureSql && ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.SQLText) == (int)RegulationSettings.RegulationDatabaseCategory.SQLText)) || tempDb.AuditCaptureSQL;
                        tempDb.AuditCaptureTrans = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.Transactions) == (int)RegulationSettings.RegulationDatabaseCategory.Transactions) || tempDb.AuditCaptureTrans;
                        tempDb.AuditSensitiveColumns = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.SensitiveColumns) == (int)RegulationSettings.RegulationDatabaseCategory.SensitiveColumns) || tempDb.AuditSensitiveColumns;
                        tempDb.AuditDataChanges = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.BeforeAfterDataChange) == (int)RegulationSettings.RegulationDatabaseCategory.BeforeAfterDataChange) || tempDb.AuditDataChanges;
                    }
                }
                else
                    db.FERPA = false;
                if (checkGDPR.Checked)
                {
                    db.GDPR = true;

                    if (_regulationSettings.TryGetValue((int)Regulation.RegulationType.GDPR, out settings))
                    {
                        tempDb.AuditDDL = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.DatabaseDefinition) == (int)RegulationSettings.RegulationDatabaseCategory.DatabaseDefinition) || tempDb.AuditDDL;
                        tempDb.AuditSecurity = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.SecurityChanges) == (int)RegulationSettings.RegulationDatabaseCategory.SecurityChanges) || tempDb.AuditSecurity;
                        tempDb.AuditAdmin = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.AdminActivity) == (int)RegulationSettings.RegulationDatabaseCategory.AdminActivity) || tempDb.AuditAdmin;
                        tempDb.AuditDML = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.DatabaseModification) == (int)RegulationSettings.RegulationDatabaseCategory.DatabaseModification) || tempDb.AuditDML;
                        tempDb.AuditSELECT = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.Select) == (int)RegulationSettings.RegulationDatabaseCategory.Select) || tempDb.AuditSELECT;

                        if (((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.FilterPassedAccess) == (int)RegulationSettings.RegulationDatabaseCategory.FilterPassedAccess) || (tempDb.AuditAccessCheck == AccessCheckFilter.SuccessOnly))
                            tempDb.AuditAccessCheck = AccessCheckFilter.SuccessOnly;
                        else if (((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.FilterFailedAccess) == (int)RegulationSettings.RegulationDatabaseCategory.FilterFailedAccess) || (tempDb.AuditAccessCheck == AccessCheckFilter.FailureOnly))
                            tempDb.AuditAccessCheck = AccessCheckFilter.FailureOnly;
                        else
                            tempDb.AuditAccessCheck = AccessCheckFilter.NoFilter;

                        tempDb.AuditCaptureSQL = (CoreConstants.AllowCaptureSql && ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.SQLText) == (int)RegulationSettings.RegulationDatabaseCategory.SQLText)) || tempDb.AuditCaptureSQL;
                        tempDb.AuditCaptureTrans = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.Transactions) == (int)RegulationSettings.RegulationDatabaseCategory.Transactions) || tempDb.AuditCaptureTrans;
                        tempDb.AuditSensitiveColumns = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.SensitiveColumns) == (int)RegulationSettings.RegulationDatabaseCategory.SensitiveColumns) || tempDb.AuditSensitiveColumns;
                        tempDb.AuditDataChanges = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.BeforeAfterDataChange) == (int)RegulationSettings.RegulationDatabaseCategory.BeforeAfterDataChange) || tempDb.AuditDataChanges;
                    }
                }
                else
                    db.GDPR = false;
            }
            //return the database setttings
            db.AuditDDL = tempDb.AuditDDL;
            db.AuditSecurity = tempDb.AuditSecurity;
            db.AuditAdmin = tempDb.AuditAdmin;
            db.AuditDML = tempDb.AuditDML;
            db.AuditSELECT = tempDb.AuditSELECT;
            db.AuditAccessCheck = tempDb.AuditAccessCheck;
            db.AuditSensitiveColumns = tempDb.AuditSensitiveColumns;
            if (db.AuditDML || db.AuditSELECT)
            {
                db.AuditCaptureSQL = tempDb.AuditCaptureSQL;
            }
            else
            {
                db.AuditCaptureSQL = false;
            }
            if (db.AuditDDL || db.AuditSecurity)
            {
                db.AuditCaptureDDL = tempDb.AuditCaptureDDL;
            }
            else
            {
                db.AuditCaptureDDL = false;
            }
            if (db.AuditDML)
            {
                db.AuditDataChanges = tempDb.AuditDataChanges;
                db.AuditCaptureTrans = tempDb.AuditCaptureTrans;
            }
            else
                db.AuditCaptureTrans = false;
        }

        private bool WriteDatabaseRecord(DatabaseRecord db, SqlTransaction transaction)
        {
            bool retval = false;

            try
            {
                if (!db.Create(transaction))
                {
                    ErrorMessage.Show(this.Text, UIConstants.Error_ErrorCreatingDatabase, DatabaseRecord.GetLastError());
                }
                else
                {
                    retval = true;
                }
            }
            finally
            {
                if (retval)
                {
                    // Register change to server and perform audit log				 
                    ServerUpdate.RegisterChange(db.SrvId, LogType.NewDatabase, db.SrvInstance, db.Name);
                }
            }
            return retval;
        }

        private bool AddSecondaryDatabases(bool isPrimary, int srvId, string srvInstance, string avgName, SqlConnection conn)
        {
            bool retval = false;
            DatabaseRecord db = new DatabaseRecord();

            try
            {
                if (!db.CopyPrimaryRecord(isPrimary, srvId, srvInstance, avgName, conn))
                {
                    ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                           String.Format("AddSecondaryDatabases: No data to copy for {0}", srvInstance), ErrorLog.Severity.Informational);
                }
                else
                {
                    retval = true;
                }
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                           String.Format("AddSecondaryDatabases: Instance {0}", srvInstance), ex, ErrorLog.Severity.Error);
            }
            return retval;
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

        #endregion
        // v5.6 SQLCM-5373
        private string GetServerLevelTrustedUserProperty()
        {
            int count = 0;

            UserList ul = new UserList();

            foreach (ListViewItem vi in lstServerTrustedUsers.Items)
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

        private void _btnBrowse_Click(object sender, EventArgs e)
        {
            OpenFileDialog frm = new OpenFileDialog();
            frm.Filter = "xml files (*.xml)|*.xml|All files (*.*)|*.*";
            frm.FilterIndex = 1;
            frm.RestoreDirectory = true;

            if (frm.ShowDialog(this) == DialogResult.OK)
            {
                _tbFile.Text = frm.FileName;
                nextButton.Enabled = true;
            }
        }

        #region Database audit Settings

        private void InitializeDBAuditSettings()
        {
            if (!_templateSvrInitializationCompleted)
            {
                this.chkUserAuditCaptureSQL.CheckedChanged -= new System.EventHandler(this.chkUserCaptureSQL_CheckedChanged);
                this.chkUserAuditCaptureSQLDatabase.CheckedChanged -= new System.EventHandler(this.chkUserCaptureSQLDatabase_CheckedChanged);
                this.ResetDbUiControls();
                this.InitializeServerDbUi();
                this.InitializeServerDbPrivilegeUsersUi();
                this.InitializeDatabaseUi();
                this.InitializeDatabasePrivilegeUsersUi();
                this.InitializeServerAuditDbDeselection();
                this.InitializeDatabaseAuditDeselection();
                InitializeDBRegulationSettings();
                this.chkUserAuditCaptureSQL.CheckedChanged += new System.EventHandler(this.chkUserCaptureSQL_CheckedChanged);
                this.chkUserAuditCaptureSQLDatabase.CheckedChanged += new System.EventHandler(this.chkUserCaptureSQLDatabase_CheckedChanged);
                _templateSvrInitializationCompleted = true;
            }
            _templateDBInitializationCompleted = true;
        }

        /// <summary>
        /// Reset Db UI Controls
        /// </summary>
        private void ResetDbUiControls()
        {
            this.ResetControls(
                this.checkPriviligedUser,
                this._checkUserDefinedEvents,
                this._checkAdministrativeActivities,
                this._checkDDL,
                this._checkSecurityChanges,
                this._checkFailedLogins,
                this._checkLogouts,
                this._chkServerAccessCheckFilter);

            this.ResetControls(this._rbAuditFailedOnly, this._rbServerAuditSuccessfulOnly);

            this.ResetControls(
                this.chkUserAuditCaptureTrans,
                this.chkUserAuditUDE,
                this.chkUserAuditAdmin,
                this.chkUserAuditCaptureSQL,
                this.chkUserAccessCheckFilter,
                this.chkUserAuditDML,
                this.chkUserAuditSELECT,
                this.chkUserAuditSecurity,
                this.chkUserAuditDDL,
                this.chkUserAuditFailedLogins,
                this.chkUserAuditLogins,
                this.chkUserAuditLogouts);

            this.ResetControls(this.rbUserAuditFailedOnly, this.rbUserAuditSuccessfulOnly);

            this.ResetControls(
                this._chkPriviligedUser,
                this._chkAuditBeforeAfter,
                this._chkAuditSensitiveColumn,
                this._chkAuditSelect,
                this._chkAuditDML,
                this._chkAuditAdminActivity,
                this._chkAuditSecurityChanges,
                this._chkAuditDDL,
                this._chkDBCaptureDDL,
                this._chkCaptureTrans,
                this._chkCaptureSQL,
                this._chkDBAccessCheckFilter);

            this.ResetControls(this._rbDBAuditFailedOnly, this._rbDBAuditSuccessfulOnly);

            this.ResetControls(
                this.GetConfigBasedCheckBox(this.chkUserAuditCaptureTransDatabase, this.chkUserAuditCaptureTrans),
                this.GetConfigBasedCheckBox(this.chkUserAuditUDEDatabase, this.chkUserAuditUDE),
                this.GetConfigBasedCheckBox(this.chkUserAuditAdminDatabase, this.chkUserAuditAdmin),
                this.GetConfigBasedCheckBox(this.chkUserAuditCaptureSQLDatabase, this.chkUserAuditCaptureSQL),
                this.GetConfigBasedCheckBox(this.chkUserAccessCheckFilterDatabase, this.chkUserAccessCheckFilter),
                this.GetConfigBasedCheckBox(this.chkUserAuditDMLDatabase, this.chkUserAuditDML),
                this.GetConfigBasedCheckBox(this.chkUserAuditSELECTDatabase, this.chkUserAuditSELECT),
                this.GetConfigBasedCheckBox(this.chkUserAuditSecurityDatabase, this.chkUserAuditSecurity),
                this.GetConfigBasedCheckBox(this.chkUserAuditDDLDatabase, this.chkUserAuditDDL),
                this.GetConfigBasedCheckBox(this.chkUserAuditFailedLoginsDatabase, this.chkUserAuditFailedLogins),
                this.GetConfigBasedCheckBox(this.chkUserAuditLoginsDatabase, this.chkUserAuditLogins),
                this.GetConfigBasedCheckBox(this.chkUserAuditLogoutsDatabase, this.chkUserAuditLogouts));

            this.ResetControls(
                this.GetConfigBasedRadioButton(this.rbUserAuditFailedOnlyDatabase, this.rbUserAuditFailedOnly),
                this.GetConfigBasedRadioButton(this.rbUserAuditSuccessfulOnlyDatabase, this.rbUserAuditSuccessfulOnly));
        }

        /// <summary>
        /// Perform Deselection based on Server Controls
        /// </summary>
        private void InitializeServerAuditDbDeselection()
        {
            DeselectionManager.GreyOutCheckboxControls(_server.AuditLogins, this.GetConfigBasedCheckBox(this.chkUserAuditLoginsDatabase, this.chkUserAuditLogins));
            // SQLCM 5563: Disable Logout with Logins for Server settings not properly configured while applying Regulation Guideline at Server Level
            if (_server.AuditLogins)
            {
                DeselectionManager.GreyOutCheckboxControls(_server.AuditLogouts, this.GetConfigBasedCheckBox(this.chkUserAuditLogoutsDatabase, this.chkUserAuditLogouts));
            }
            DeselectionManager.GreyOutCheckboxControls(_server.AuditFailedLogins, this.GetConfigBasedCheckBox(this.chkUserAuditFailedLoginsDatabase, this.chkUserAuditFailedLogins));
            DeselectionManager.GreyOutCheckboxControls(_server.AuditSecurity, _chkAuditSecurityChanges, this.GetConfigBasedCheckBox(this.chkUserAuditSecurityDatabase, this.chkUserAuditSecurity));
            DeselectionManager.GreyOutCheckboxControls(_server.AuditDDL, _chkAuditDDL, this.GetConfigBasedCheckBox(this.chkUserAuditDDLDatabase, this.chkUserAuditDDL));
            DeselectionManager.GreyOutCheckboxControls(_server.AuditAdmin, this._chkAuditAdminActivity, this.GetConfigBasedCheckBox(this.chkUserAuditAdminDatabase, this.chkUserAuditAdmin));
            DeselectionManager.GreyOutCheckboxControls(_server.AuditDML, _chkAuditDML, this.GetConfigBasedCheckBox(this.chkUserAuditDMLDatabase, this.chkUserAuditDML));
            DeselectionManager.GreyOutCheckboxControls(_server.AuditUDE, this.GetConfigBasedCheckBox(this.chkUserAuditUDEDatabase, this.chkUserAuditUDE));
            switch (_server.AuditAccessCheck)
            {
                case AccessCheckFilter.NoFilter:
                    break;
                case AccessCheckFilter.SuccessOnly:
                    DeselectionManager.GreyOutCheckboxControls(true, _chkDBAccessCheckFilter, this.GetConfigBasedCheckBox(this.chkUserAccessCheckFilterDatabase, this.chkUserAccessCheckFilter));
                    DeselectionManager.GreyOutRadioButtonControls(true, _rbDBAuditSuccessfulOnly, this.GetConfigBasedRadioButton(this.rbUserAuditSuccessfulOnlyDatabase, this.rbUserAuditSuccessfulOnly));
                    DeselectionManager.DisableRadioButtonControls(true, _rbDBAuditFailedOnly, this.GetConfigBasedRadioButton(this.rbUserAuditFailedOnlyDatabase, this.rbUserAuditFailedOnly));
                    break;
                case AccessCheckFilter.FailureOnly:
                    DeselectionManager.GreyOutCheckboxControls(true, _chkDBAccessCheckFilter, this.GetConfigBasedCheckBox(this.chkUserAccessCheckFilterDatabase, this.chkUserAccessCheckFilter));
                    DeselectionManager.DisableRadioButtonControls(true, _rbDBAuditSuccessfulOnly, this.GetConfigBasedRadioButton(this.rbUserAuditSuccessfulOnlyDatabase, this.rbUserAuditSuccessfulOnly));
                    DeselectionManager.GreyOutRadioButtonControls(true, _rbDBAuditFailedOnly, this.GetConfigBasedRadioButton(this.rbUserAuditFailedOnlyDatabase, this.rbUserAuditFailedOnly));
                    break;
            }

            DeselectionManager.GreyOutCheckboxControls(_server.AuditUserLogins, this.GetConfigBasedCheckBox(this.chkUserAuditLoginsDatabase, this.chkUserAuditLogins));
            // SQLCM 5563: Disable Logout with Logins for Server settings not properly configured while applying Regulation Guideline at Server Level
            if (_server.AuditUserLogins)
            {
                DeselectionManager.GreyOutCheckboxControls(_server.AuditUserLogouts, this.GetConfigBasedCheckBox(this.chkUserAuditLogoutsDatabase, this.chkUserAuditLogouts));
            }
            DeselectionManager.GreyOutCheckboxControls(_server.AuditUserFailedLogins, this.GetConfigBasedCheckBox(this.chkUserAuditFailedLoginsDatabase, this.chkUserAuditFailedLogins));
            DeselectionManager.GreyOutCheckboxControls(_server.AuditUserSecurity, this.GetConfigBasedCheckBox(this.chkUserAuditSecurityDatabase, this.chkUserAuditSecurity));
            DeselectionManager.GreyOutCheckboxControls(_server.AuditUserDDL, this.GetConfigBasedCheckBox(this.chkUserAuditDDLDatabase, this.chkUserAuditDDL));
            DeselectionManager.GreyOutCheckboxControls(_server.AuditUserAdmin, this.GetConfigBasedCheckBox(this.chkUserAuditAdminDatabase, this.chkUserAuditAdmin));
            DeselectionManager.GreyOutCheckboxControls(_server.AuditUserCaptureSQL, this.GetConfigBasedCheckBox(this.chkUserAuditCaptureSQLDatabase, this.chkUserAuditCaptureSQL));
            DeselectionManager.GreyOutCheckboxControls(_server.AuditUserCaptureDDL, this.GetConfigBasedCheckBox(this.chkUserCaptureDDLDatabase, this.chkUserCaptureDDL));
            DeselectionManager.GreyOutCheckboxControls(_server.AuditUserCaptureTrans, this.GetConfigBasedCheckBox(this.chkUserAuditCaptureTransDatabase, this.chkUserAuditCaptureTrans));
            DeselectionManager.GreyOutCheckboxControls(_server.AuditUserSELECT, this.GetConfigBasedCheckBox(this.chkUserAuditSELECTDatabase, this.chkUserAuditSELECT));
            DeselectionManager.GreyOutCheckboxControls(_server.AuditUserDML, this.GetConfigBasedCheckBox(this.chkUserAuditDMLDatabase, this.chkUserAuditDML));
            DeselectionManager.GreyOutCheckboxControls(_server.AuditUserUDE, this.GetConfigBasedCheckBox(this.chkUserAuditUDEDatabase, this.chkUserAuditUDE));

            if (_server.AuditAccessCheck == AccessCheckFilter.NoFilter && this.GetConfigBasedCheckBox(this.chkUserAccessCheckFilterDatabase, this.chkUserAccessCheckFilter) != null)
            {
                switch (_server.AuditUserAccessCheck)
                {
                    case AccessCheckFilter.NoFilter: break;
                    case AccessCheckFilter.SuccessOnly:
                        DeselectionManager.GreyOutCheckboxControls(true, this.GetConfigBasedCheckBox(this.chkUserAccessCheckFilterDatabase, this.chkUserAccessCheckFilter));
                        DeselectionManager.GreyOutRadioButtonControls(true, this.GetConfigBasedRadioButton(this.rbUserAuditSuccessfulOnlyDatabase, this.rbUserAuditSuccessfulOnly));
                        DeselectionManager.DisableRadioButtonControls(true, this.GetConfigBasedRadioButton(this.rbUserAuditFailedOnlyDatabase, this.rbUserAuditFailedOnly));
                        break;
                    case AccessCheckFilter.FailureOnly:
                        DeselectionManager.GreyOutCheckboxControls(true, this.GetConfigBasedCheckBox(this.chkUserAccessCheckFilterDatabase, this.chkUserAccessCheckFilter));
                        DeselectionManager.DisableRadioButtonControls(true, this.GetConfigBasedRadioButton(this.rbUserAuditSuccessfulOnlyDatabase, this.rbUserAuditSuccessfulOnly));
                        DeselectionManager.GreyOutRadioButtonControls(true, this.GetConfigBasedRadioButton(this.rbUserAuditFailedOnlyDatabase, this.rbUserAuditFailedOnly));
                        break;
                }
            }

        }

        /// <summary>
        /// Initialize Server Database UI
        /// </summary>
        private void InitializeServerDbUi()
        {
            if (_templateSvrInitializationCompleted)
                return;

            DeselectionManager.SetAdditiveCheckbox(this._server.AuditAdmin, _chkAuditAdminActivity, this.GetConfigBasedCheckBox(this.chkUserAuditAdminDatabase, this.chkUserAuditAdmin));

            DeselectionManager.SetAdditiveCheckbox(this._server.AuditDDL, _chkAuditDDL, this.GetConfigBasedCheckBox(this.chkUserAuditDDLDatabase, this.chkUserAuditDDL));

            DeselectionManager.SetAdditiveCheckbox(this._server.AuditFailedLogins, this.GetConfigBasedCheckBox(this.chkUserAuditFailedLoginsDatabase, this.chkUserAuditFailedLogins));

            DeselectionManager.SetAdditiveCheckbox(this._server.AuditLogins, this.GetConfigBasedCheckBox(this.chkUserAuditLoginsDatabase, this.chkUserAuditLogins));
            // SQLCM 5563: Disable Logout with Logins for Server settings not properly configured while applying Regulation Guideline at Server Level
            if (_server.AuditLogins)
            {
                DeselectionManager.SetAdditiveCheckbox(this._server.AuditLogouts, this.GetConfigBasedCheckBox(this.chkUserAuditLogoutsDatabase, this.chkUserAuditLogouts));
            }

            DeselectionManager.SetAdditiveCheckbox(this._server.AuditSecurity, this.GetConfigBasedCheckBox(this.chkUserAuditSecurityDatabase, this.chkUserAuditSecurity));

            DeselectionManager.SetAdditiveCheckbox(this._server.AuditUDE, this.GetConfigBasedCheckBox(this.chkUserAuditUDEDatabase, this.chkUserAuditUDE));

            if (_server.AuditAccessCheck == AccessCheckFilter.FailureOnly)
            {
                CheckRb(this.GetConfigBasedRadioButton(this.rbUserAuditFailedOnlyDatabase, this.rbUserAuditFailedOnly), true);
                _rbAuditFailedOnly.Checked = true;

                CheckCb(this.GetConfigBasedCheckBox(this.chkUserAccessCheckFilterDatabase, this.chkUserAccessCheckFilter), true);
                _chkServerAccessCheckFilter.Checked = true;

                EnableRb(this.GetConfigBasedRadioButton(this.rbUserAuditFailedOnlyDatabase, this.rbUserAuditFailedOnly), true);
                _rbAuditFailedOnly.Enabled = true;

                EnableRb(this.GetConfigBasedRadioButton(this.rbUserAuditSuccessfulOnlyDatabase, this.rbUserAuditSuccessfulOnly), true);
                _rbServerAuditSuccessfulOnly.Enabled = true;
            }
            else if (_server.AuditAccessCheck == AccessCheckFilter.SuccessOnly)
            {
                CheckRb(this.GetConfigBasedRadioButton(this.rbUserAuditSuccessfulOnlyDatabase, this.rbUserAuditSuccessfulOnly), true);
                _rbDBAuditSuccessfulOnly.Checked = true;

                CheckCb(this.GetConfigBasedCheckBox(this.chkUserAccessCheckFilterDatabase, this.chkUserAccessCheckFilter), true);
                _chkDBAccessCheckFilter.Checked = true;

                EnableRb(this.GetConfigBasedRadioButton(this.rbUserAuditFailedOnlyDatabase, this.rbUserAuditFailedOnly), true);
                _rbDBAuditFailedOnly.Enabled = true;

                EnableRb(this.GetConfigBasedRadioButton(this.rbUserAuditSuccessfulOnlyDatabase, this.rbUserAuditSuccessfulOnly), true);
                _rbDBAuditSuccessfulOnly.Enabled = true;
            }
            else
            {
                CheckCb(this.GetConfigBasedCheckBox(this.chkUserAccessCheckFilterDatabase, this.chkUserAccessCheckFilter), false);
                _chkDBAccessCheckFilter.Checked = false;

                EnableRb(this.GetConfigBasedRadioButton(this.rbUserAuditFailedOnlyDatabase, this.rbUserAuditFailedOnly), false);
                _rbDBAuditFailedOnly.Enabled = false;

                EnableRb(this.GetConfigBasedRadioButton(this.rbUserAuditSuccessfulOnlyDatabase, this.rbUserAuditSuccessfulOnly), false);
                _rbDBAuditSuccessfulOnly.Enabled = false;
            }
        }

        /// <summary>
        /// Initialize Server Database Privilege Users
        /// </summary>
        private void InitializeServerDbPrivilegeUsersUi()
        {
            if (_templateSvrInitializationCompleted)
                return;
            DeselectionManager.SetAdditiveCheckbox(this._server.AuditUserAdmin, this.GetConfigBasedCheckBox(this.chkUserAuditAdminDatabase, this.chkUserAuditAdmin));
            DeselectionManager.SetAdditiveCheckbox(this._server.AuditUserCaptureSQL, this.GetConfigBasedCheckBox(this.chkUserAuditCaptureSQLDatabase, this.chkUserAuditCaptureSQL));
            DeselectionManager.SetAdditiveCheckbox(this._server.AuditUserCaptureDDL, this.GetConfigBasedCheckBox(this.chkUserCaptureDDLDatabase, this.chkUserCaptureDDL));
            DeselectionManager.SetAdditiveCheckbox(this._server.AuditUserCaptureTrans, this.GetConfigBasedCheckBox(this.chkUserAuditCaptureTransDatabase, this.chkUserAuditCaptureTrans));
            DeselectionManager.SetAdditiveCheckbox(this._server.AuditUserDDL, this.GetConfigBasedCheckBox(this.chkUserAuditDDLDatabase, this.chkUserAuditDDL));
            DeselectionManager.SetAdditiveCheckbox(this._server.AuditUserDML, this.GetConfigBasedCheckBox(this.chkUserAuditDMLDatabase, this.chkUserAuditDML));
            DeselectionManager.SetAdditiveCheckbox(this._server.AuditUserFailedLogins, this.GetConfigBasedCheckBox(this.chkUserAuditFailedLoginsDatabase, this.chkUserAuditFailedLogins));
            DeselectionManager.SetAdditiveCheckbox(this._server.AuditUserLogins, this.GetConfigBasedCheckBox(this.chkUserAuditLoginsDatabase, this.chkUserAuditLogins));
            // SQLCM 5563: Disable Logout with Logins for Server settings not properly configured while applying Regulation Guideline at Server Level
            if (this._server.AuditUserLogins)
            {
                DeselectionManager.SetAdditiveCheckbox(this._server.AuditUserLogouts, this.GetConfigBasedCheckBox(this.chkUserAuditLogoutsDatabase, this.chkUserAuditLogouts));
            }
            DeselectionManager.SetAdditiveCheckbox(this._server.AuditUserSecurity, this.GetConfigBasedCheckBox(this.chkUserAuditSecurityDatabase, this.chkUserAuditSecurity));
            DeselectionManager.SetAdditiveCheckbox(this._server.AuditUserSELECT, this.GetConfigBasedCheckBox(this.chkUserAuditSELECTDatabase, this.chkUserAuditSELECT));
            DeselectionManager.SetAdditiveCheckbox(this._server.AuditUserUDE, this.GetConfigBasedCheckBox(this.chkUserAuditUDEDatabase, this.chkUserAuditUDE));

            if (_server.AuditAccessCheck == AccessCheckFilter.NoFilter)
            {
                if (_server.AuditUserAccessCheck == AccessCheckFilter.FailureOnly)
                {
                    CheckRb(this.GetConfigBasedRadioButton(this.rbUserAuditFailedOnlyDatabase, this.rbUserAuditFailedOnly), true);
                    CheckCb(this.GetConfigBasedCheckBox(this.chkUserAccessCheckFilterDatabase, this.chkUserAccessCheckFilter), true);
                    EnableRb(this.GetConfigBasedRadioButton(this.rbUserAuditFailedOnlyDatabase, this.rbUserAuditFailedOnly), true);
                    EnableRb(this.GetConfigBasedRadioButton(this.rbUserAuditSuccessfulOnlyDatabase, this.rbUserAuditSuccessfulOnly), true);
                }
                else if (_server.AuditUserAccessCheck == AccessCheckFilter.SuccessOnly)
                {
                    CheckRb(this.GetConfigBasedRadioButton(this.rbUserAuditSuccessfulOnlyDatabase, this.rbUserAuditSuccessfulOnly), true);
                    CheckCb(this.GetConfigBasedCheckBox(this.chkUserAccessCheckFilterDatabase, this.chkUserAccessCheckFilter), true);
                    EnableRb(this.GetConfigBasedRadioButton(this.rbUserAuditFailedOnlyDatabase, this.rbUserAuditFailedOnly), true);
                    EnableRb(this.GetConfigBasedRadioButton(this.rbUserAuditSuccessfulOnlyDatabase, this.rbUserAuditSuccessfulOnly), true);
                }
                else
                {
                    CheckCb(this.GetConfigBasedCheckBox(this.chkUserAccessCheckFilterDatabase, this.chkUserAccessCheckFilter), false);
                    EnableRb(this.GetConfigBasedRadioButton(this.rbUserAuditFailedOnlyDatabase, this.rbUserAuditFailedOnly), false);
                    EnableRb(this.GetConfigBasedRadioButton(this.rbUserAuditSuccessfulOnlyDatabase, this.rbUserAuditSuccessfulOnly), false);
                }
            }
        }

        /// <summary>
        /// Initialize Database UI
        /// </summary>
        private void InitializeDatabaseUi()
        {
            if (_templateSvrInitializationCompleted) return;


            // Add for Database Privilege Users Settings after pulling latest
            if (this._databases != null && this._databases.Count > 0)
            {
                var dbRecord = this._databases[0];
                DeselectionManager.SetAdditiveCheckbox(dbRecord.AuditSELECT, this._chkAuditSelect, this.GetConfigBasedCheckBox(this.chkUserAuditSELECTDatabase, this.chkUserAuditSELECT));
                DeselectionManager.SetAdditiveCheckbox(dbRecord.AuditDML, this._chkAuditDML, this.GetConfigBasedCheckBox(this.chkUserAuditDMLDatabase, this.chkUserAuditDML));
                DeselectionManager.SetAdditiveCheckbox(dbRecord.AuditAdmin, this._chkAuditAdminActivity, this.GetConfigBasedCheckBox(this.chkUserAuditAdminDatabase, this.chkUserAuditAdmin));
                DeselectionManager.SetAdditiveCheckbox(dbRecord.AuditSecurity, this._chkAuditSecurityChanges, this.GetConfigBasedCheckBox(this.chkUserAuditSecurityDatabase, this.chkUserAuditSecurity));
                DeselectionManager.SetAdditiveCheckbox(dbRecord.AuditDDL, this._chkAuditDDL, this.GetConfigBasedCheckBox(this.chkUserAuditDDLDatabase, this.chkUserAuditDDL));
                DeselectionManager.SetAdditiveCheckbox(dbRecord.AuditCaptureDDL, this._chkDBCaptureDDL, this.GetConfigBasedCheckBox(this.chkUserCaptureDDLDatabase, this.chkUserCaptureDDL));
                DeselectionManager.SetAdditiveCheckbox(dbRecord.AuditCaptureTrans, this._chkCaptureTrans, this.GetConfigBasedCheckBox(this.chkUserAuditCaptureTransDatabase, this.chkUserAuditCaptureTrans));
                this._chkCaptureSQL.CheckedChanged -= new System.EventHandler(this._chkCaptureSQL_CheckedChanged);
                DeselectionManager.SetAdditiveCheckbox(dbRecord.AuditCaptureSQL, this._chkCaptureSQL, this.GetConfigBasedCheckBox(this.chkUserAuditCaptureSQLDatabase, this.chkUserAuditCaptureSQL));
                this._chkCaptureSQL.CheckedChanged += new System.EventHandler(this._chkCaptureSQL_CheckedChanged);

                if (_server.AuditAccessCheck == AccessCheckFilter.NoFilter)
                {
                    if (dbRecord.AuditAccessCheck == AccessCheckFilter.FailureOnly)
                    {
                        CheckRb(this.GetConfigBasedRadioButton(this.rbUserAuditFailedOnlyDatabase, this.rbUserAuditFailedOnly), true);
                        CheckCb(this.GetConfigBasedCheckBox(this.chkUserAccessCheckFilterDatabase, this.chkUserAccessCheckFilter), true);
                        EnableRb(this.GetConfigBasedRadioButton(this.rbUserAuditFailedOnlyDatabase, this.rbUserAuditFailedOnly), true);
                        EnableRb(this.GetConfigBasedRadioButton(this.rbUserAuditSuccessfulOnlyDatabase, this.rbUserAuditSuccessfulOnly), true);
                        _rbDBAuditFailedOnly.Checked = true;
                        _chkDBAccessCheckFilter.Checked = true;
                        _rbDBAuditFailedOnly.Enabled = true;
                        _rbDBAuditSuccessfulOnly.Enabled = true;
                    }
                    else if (dbRecord.AuditAccessCheck == AccessCheckFilter.SuccessOnly)
                    {
                        CheckRb(this.GetConfigBasedRadioButton(this.rbUserAuditSuccessfulOnlyDatabase, this.rbUserAuditSuccessfulOnly), true);
                        CheckCb(this.GetConfigBasedCheckBox(this.chkUserAccessCheckFilterDatabase, this.chkUserAccessCheckFilter), true);
                        EnableRb(this.GetConfigBasedRadioButton(this.rbUserAuditFailedOnlyDatabase, this.rbUserAuditFailedOnly), true);
                        EnableRb(this.GetConfigBasedRadioButton(this.rbUserAuditSuccessfulOnlyDatabase, this.rbUserAuditSuccessfulOnly), true);
                        _chkDBAccessCheckFilter.Checked = true;
                        _rbDBAuditSuccessfulOnly.Checked = true;
                        _rbDBAuditFailedOnly.Enabled = true;
                        _rbDBAuditSuccessfulOnly.Enabled = true;
                    }
                    else
                    {
                        _chkDBAccessCheckFilter.Checked = false;
                        _rbDBAuditFailedOnly.Enabled = false;
                        _rbDBAuditSuccessfulOnly.Enabled = false;
                    }
                }
            }
        }

        /// <summary>
        /// Initialize Database Deselection
        /// </summary>
        private void InitializeDatabaseAuditDeselection()
        {
            if (_templateSvrInitializationCompleted) return;

            // Add for Database Privilege Users Settings after pulling latest
            if (this._databases != null && this._databases.Count > 0)
            {
                var dbRecord = this._databases[0];
                DeselectionManager.GreyOutCheckboxControls(dbRecord.AuditSELECT, this._chkAuditSelect, this.GetConfigBasedCheckBox(this.chkUserAuditSELECTDatabase, this.chkUserAuditSELECT));
                DeselectionManager.GreyOutCheckboxControls(dbRecord.AuditDML, this._chkAuditDML, this.GetConfigBasedCheckBox(this.chkUserAuditDMLDatabase, this.chkUserAuditDML));
                DeselectionManager.GreyOutCheckboxControls(dbRecord.AuditAdmin, this._chkAuditAdminActivity, this.GetConfigBasedCheckBox(this.chkUserAuditAdminDatabase, this.chkUserAuditAdmin));
                DeselectionManager.GreyOutCheckboxControls(dbRecord.AuditSecurity, this._chkAuditSecurityChanges, this.GetConfigBasedCheckBox(this.chkUserAuditSecurityDatabase, this.chkUserAuditSecurity));
                DeselectionManager.GreyOutCheckboxControls(dbRecord.AuditDDL, this._chkAuditDDL, this.GetConfigBasedCheckBox(this.chkUserAuditDDLDatabase, this.chkUserAuditDDL));
                DeselectionManager.GreyOutCheckboxControls(dbRecord.AuditCaptureDDL, this._chkDBCaptureDDL, this.GetConfigBasedCheckBox(this.chkUserCaptureDDLDatabase, this.chkUserCaptureDDL));
                DeselectionManager.GreyOutCheckboxControls(dbRecord.AuditCaptureTrans, this._chkCaptureTrans, this.GetConfigBasedCheckBox(this.chkUserAuditCaptureTransDatabase, this.chkUserAuditCaptureTrans));
                DeselectionManager.GreyOutCheckboxControls(dbRecord.AuditCaptureSQL, this._chkCaptureSQL, this.GetConfigBasedCheckBox(this.chkUserAuditCaptureSQLDatabase, this.chkUserAuditCaptureSQL));
                if (_server.AuditAccessCheck == AccessCheckFilter.NoFilter && _server.AuditUserAccessCheck == AccessCheckFilter.NoFilter)
                {

                    switch (dbRecord.AuditAccessCheck)
                    {
                        case AccessCheckFilter.NoFilter:
                            break;
                        case AccessCheckFilter.SuccessOnly:
                            DeselectionManager.GreyOutCheckboxControls(true, _chkDBAccessCheckFilter, this.GetConfigBasedCheckBox(this.chkUserAccessCheckFilterDatabase, this.chkUserAccessCheckFilter));
                            DeselectionManager.GreyOutRadioButtonControls(true, _rbDBAuditSuccessfulOnly, this.GetConfigBasedRadioButton(this.rbUserAuditSuccessfulOnlyDatabase, this.rbUserAuditSuccessfulOnly));
                            DeselectionManager.DisableRadioButtonControls(true, _rbDBAuditFailedOnly, this.GetConfigBasedRadioButton(this.rbUserAuditFailedOnlyDatabase, this.rbUserAuditFailedOnly));
                            break;
                        case AccessCheckFilter.FailureOnly:
                            DeselectionManager.GreyOutCheckboxControls(true, _chkDBAccessCheckFilter, this.GetConfigBasedCheckBox(this.chkUserAccessCheckFilterDatabase, this.chkUserAccessCheckFilter));
                            DeselectionManager.DisableRadioButtonControls(true, _rbDBAuditSuccessfulOnly, this.GetConfigBasedRadioButton(this.rbUserAuditSuccessfulOnlyDatabase, this.rbUserAuditSuccessfulOnly));
                            DeselectionManager.GreyOutRadioButtonControls(true, _rbDBAuditFailedOnly, this.GetConfigBasedRadioButton(this.rbUserAuditFailedOnlyDatabase, this.rbUserAuditFailedOnly));
                            break;
                    }
                }
                else
                {
                    {
                        switch (_server.AuditAccessCheck)
                        {
                            case AccessCheckFilter.NoFilter:
                                switch (_server.AuditUserAccessCheck)
                                {
                                    case AccessCheckFilter.SuccessOnly:
                                        DeselectionManager.GreyOutCheckboxControls(true, _chkDBAccessCheckFilter, this.GetConfigBasedCheckBox(this.chkUserAccessCheckFilterDatabase, this.chkUserAccessCheckFilter));
                                        DeselectionManager.GreyOutRadioButtonControls(true, _rbDBAuditSuccessfulOnly, this.GetConfigBasedRadioButton(this.rbUserAuditSuccessfulOnlyDatabase, this.rbUserAuditSuccessfulOnly));
                                        DeselectionManager.DisableRadioButtonControls(true, _rbDBAuditFailedOnly, this.GetConfigBasedRadioButton(this.rbUserAuditFailedOnlyDatabase, this.rbUserAuditFailedOnly));
                                        break;
                                    case AccessCheckFilter.FailureOnly:
                                        DeselectionManager.GreyOutCheckboxControls(true, _chkDBAccessCheckFilter, this.GetConfigBasedCheckBox(this.chkUserAccessCheckFilterDatabase, this.chkUserAccessCheckFilter));
                                        DeselectionManager.DisableRadioButtonControls(true, _rbDBAuditSuccessfulOnly, this.GetConfigBasedRadioButton(this.rbUserAuditSuccessfulOnlyDatabase, this.rbUserAuditSuccessfulOnly));
                                        DeselectionManager.GreyOutRadioButtonControls(true, _rbDBAuditFailedOnly, this.GetConfigBasedRadioButton(this.rbUserAuditFailedOnlyDatabase, this.rbUserAuditFailedOnly));
                                        break;
                                }

                                break;
                            case AccessCheckFilter.SuccessOnly:
                                DeselectionManager.GreyOutCheckboxControls(true, _chkDBAccessCheckFilter, this.GetConfigBasedCheckBox(this.chkUserAccessCheckFilterDatabase, this.chkUserAccessCheckFilter));
                                DeselectionManager.GreyOutRadioButtonControls(true, _rbDBAuditSuccessfulOnly, this.GetConfigBasedRadioButton(this.rbUserAuditSuccessfulOnlyDatabase, this.rbUserAuditSuccessfulOnly));
                                DeselectionManager.DisableRadioButtonControls(true, _rbDBAuditFailedOnly, this.GetConfigBasedRadioButton(this.rbUserAuditFailedOnlyDatabase, this.rbUserAuditFailedOnly));
                                break;
                            case AccessCheckFilter.FailureOnly:
                                DeselectionManager.GreyOutCheckboxControls(true, _chkDBAccessCheckFilter, this.GetConfigBasedCheckBox(this.chkUserAccessCheckFilterDatabase, this.chkUserAccessCheckFilter));
                                DeselectionManager.DisableRadioButtonControls(true, _rbDBAuditSuccessfulOnly, this.GetConfigBasedRadioButton(this.rbUserAuditSuccessfulOnlyDatabase, this.rbUserAuditSuccessfulOnly));
                                DeselectionManager.GreyOutRadioButtonControls(true, _rbDBAuditFailedOnly, this.GetConfigBasedRadioButton(this.rbUserAuditFailedOnlyDatabase, this.rbUserAuditFailedOnly));
                                break;
                        }
                    }
                }
            }
            _deselectionManager.RegisterCheckbox(_chkDBAccessCheckFilter, DeselectControls.DbFilterEvents);
            _deselectionManager.RegisterRadioButton(_rbDBAuditFailedOnly, DeselectControls.DbFilterEventsFailedOnly);
            _deselectionManager.RegisterRadioButton(_rbDBAuditSuccessfulOnly, DeselectControls.DbFilterEventsPassOnly);
            _deselectionManager.RegisterCheckbox(_chkCaptureSQL, DeselectControls.DbCaptureSqlDmlSelect);
            _deselectionManager.RegisterCheckbox(_chkCaptureTrans, DeselectControls.DbCaptureSqlTransactionStatus);
            _deselectionManager.RegisterCheckbox(_chkDBCaptureDDL, DeselectControls.DbCaptureSqlDdlSecurity);
            _deselectionManager.RegisterCheckbox(_chkAuditDDL, DeselectControls.DbDatabaseDefinition);
            _deselectionManager.RegisterCheckbox(_chkAuditSecurityChanges, DeselectControls.DbSecurityChanges);
            _deselectionManager.RegisterCheckbox(_chkAuditAdminActivity, DeselectControls.DbAdministrativeActivities);
            _deselectionManager.RegisterCheckbox(_chkAuditDML, DeselectControls.DbDatabaseModifications);
            _deselectionManager.RegisterCheckbox(_chkAuditSelect, DeselectControls.DbDatabaseSelect);
        }

        private void InitializeServerAuditSettings()
        {
            this.ResetServerUiControls();

            this.chkUserAuditCaptureSQL.CheckedChanged -= new System.EventHandler(this.chkUserCaptureSQL_CheckedChanged);
            this.chkUserAuditCaptureSQLDatabase.CheckedChanged -= new System.EventHandler(this.chkUserCaptureSQLDatabase_CheckedChanged);
            this.InitializeServerUi();
            this.InitializeServerPrivilegeUsersUi();
            this.InitializeDatabaseUi();
            this.InitializeDatabasePrivilegeUsersUi();
            this.InitializeServerAuditDeselection();
            this.InitializeDatabaseAuditDeselection();
            InitializeServerRegulationSettings(_server);
            InitializeServerPrivilegedUserSettings();
            InitializeDBRegulationSettings();
            this.chkUserAuditCaptureSQL.CheckedChanged += new System.EventHandler(this.chkUserCaptureSQL_CheckedChanged);
            this.chkUserAuditCaptureSQLDatabase.CheckedChanged += new System.EventHandler(this.chkUserCaptureSQLDatabase_CheckedChanged);
            _templateSvrInitializationCompleted = true;
        }

        // SQLC-5372
        private void InitializeServerPrivilegedUserSettings()
        {
            ServerRecord tempServer = new ServerRecord();
            RegulationSettings settings;
            bool _privUserServerCheck = false;
            if (checkGDPR.Checked)
            {
                if (_regulationSettings.TryGetValue((int)Regulation.RegulationType.GDPR, out settings))
                {
                    _privUserServerCheck = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.PrivelegedUsers) == (int)RegulationSettings.RegulationServerCategory.PrivelegedUsers) || _privUserServerCheck;
                    // SQlCM 5372 - GDPR 
                    if (_privUserServerCheck)
                    {
                        DeselectionManager.SetAdditiveCheckbox(_privUserServerCheck, chkUserAuditDML);
                        tempServer.AuditUserAccessCheck = AccessCheckFilter.SuccessOnly;
                    }
                }
                if (_server.AuditAccessCheck == AccessCheckFilter.NoFilter && this._server.AuditUserAccessCheck == AccessCheckFilter.NoFilter)
                {
                    if (tempServer.AuditAccessCheck == AccessCheckFilter.NoFilter)
                    {
                        chkUserAccessCheckFilter.Checked = false;
                        rbUserAuditFailedOnly.Enabled = false;
                        rbUserAuditSuccessfulOnly.Enabled = false;
                    }
                    else
                    {
                        chkUserAccessCheckFilter.Checked = true;
                        rbUserAuditFailedOnly.Enabled = true;
                        rbUserAuditSuccessfulOnly.Enabled = true;
                        if (tempServer.AuditAccessCheck == AccessCheckFilter.FailureOnly)
                        {
                            rbUserAuditFailedOnly.Checked = true;
                        }
                        else
                        {
                            rbUserAuditSuccessfulOnly.Checked = true;
                        }
                    }
                }
            }
        }

        private void ResetServerUiControls()
        {
            this.ResetControls(
                this.checkPriviligedUser,
                this._checkUserDefinedEvents,
                this._checkAdministrativeActivities,
                this._checkDDL,
                this._checkSecurityChanges,
                this._checkFailedLogins,
                this._checkLogouts,
                this._chkServerAccessCheckFilter);

            this.ResetControls(this._rbAuditFailedOnly, this._rbServerAuditSuccessfulOnly);

            this.ResetControls(
                this.chkUserAuditCaptureTrans,
                this.chkUserAuditUDE,
                this.chkUserAuditAdmin,
                this.chkUserAuditCaptureSQL,
                this.chkUserAccessCheckFilter,
                this.chkUserAuditDML,
                this.chkUserAuditSELECT,
                this.chkUserAuditSecurity,
                this.chkUserAuditDDL,
                this.chkUserAuditFailedLogins,
                this.chkUserAuditLogins,
                this.chkUserAuditLogouts);

            this.ResetControls(this.rbUserAuditFailedOnly, this.rbUserAuditSuccessfulOnly);

            this.ResetControls(
                this._chkPriviligedUser,
                this._chkAuditBeforeAfter,
                this._chkAuditSensitiveColumn,
                this._chkAuditSelect,
                this._chkAuditDML,
                this._chkAuditAdminActivity,
                this._chkAuditSecurityChanges,
                this._chkAuditDDL,
                this._chkDBCaptureDDL,
                this._chkCaptureTrans,
                this._chkCaptureSQL,
                this._chkDBAccessCheckFilter);

            this.ResetControls(this._rbDBAuditFailedOnly, this._rbDBAuditSuccessfulOnly);

            this.ResetControls(
                this.GetConfigBasedCheckBox(this.chkUserAuditCaptureTransDatabase, this.chkUserAuditCaptureTrans),
                this.GetConfigBasedCheckBox(this.chkUserAuditUDEDatabase, this.chkUserAuditUDE),
                this.GetConfigBasedCheckBox(this.chkUserAuditAdminDatabase, this.chkUserAuditAdmin),
                this.GetConfigBasedCheckBox(this.chkUserAuditCaptureSQLDatabase, this.chkUserAuditCaptureSQL),
                this.GetConfigBasedCheckBox(this.chkUserAccessCheckFilterDatabase, this.chkUserAccessCheckFilter),
                this.GetConfigBasedCheckBox(this.chkUserAuditDMLDatabase, this.chkUserAuditDML),
                this.GetConfigBasedCheckBox(this.chkUserAuditSELECTDatabase, this.chkUserAuditSELECT),
                this.GetConfigBasedCheckBox(this.chkUserAuditSecurityDatabase, this.chkUserAuditSecurity),
                this.GetConfigBasedCheckBox(this.chkUserAuditDDLDatabase, this.chkUserAuditDDL),
                this.GetConfigBasedCheckBox(this.chkUserAuditFailedLoginsDatabase, this.chkUserAuditFailedLogins),
                this.GetConfigBasedCheckBox(this.chkUserAuditLoginsDatabase, this.chkUserAuditLogins),
                this.GetConfigBasedCheckBox(this.chkUserAuditLogoutsDatabase, this.chkUserAuditLogouts));

            this.ResetControls(
                this.GetConfigBasedRadioButton(this.rbUserAuditFailedOnlyDatabase, this.rbUserAuditFailedOnly),
                this.GetConfigBasedRadioButton(this.rbUserAuditSuccessfulOnlyDatabase, this.rbUserAuditSuccessfulOnly));
        }

        /// <summary>
        /// Reset Controls
        /// </summary>
        /// <param name="radioButtonControls"></param>
        private void ResetControls(params RadioButton[] radioButtonControls)
        {
            foreach (var radioButton in radioButtonControls)
            {
                if (radioButton == null)
                {
                    continue;
                }
                radioButton.Enabled = false;
            }
        }

        private void ResetControls(params CheckBox[] checkBoxControls)
        {
            foreach (var checkBox in checkBoxControls)
            {
                if (checkBox == null)
                {
                    continue;
                }
                checkBox.Checked = false;
            }
        }

        private void InitializeServerUi()
        {
            if (_templateSvrInitializationCompleted)
                return;

            DeselectionManager.SetAdditiveCheckbox(this._server.AuditAdmin, _checkAdministrativeActivities, this.chkUserAuditAdmin, _chkAuditAdminActivity, this.GetConfigBasedCheckBox(this.chkUserAuditAdminDatabase, this.chkUserAuditAdmin));

            DeselectionManager.SetAdditiveCheckbox(this._server.AuditDDL, _chkAuditDDL, this.chkUserAuditDDL, this.GetConfigBasedCheckBox(this.chkUserAuditDDLDatabase, this.chkUserAuditDDL));

            DeselectionManager.SetAdditiveCheckbox(this._server.AuditFailedLogins, this.chkUserAuditFailedLogins, _checkFailedLogins, this.GetConfigBasedCheckBox(this.chkUserAuditFailedLoginsDatabase, this.chkUserAuditFailedLogins));

            DeselectionManager.SetAdditiveCheckbox(this._server.AuditLogins, _checkLogins, this.chkUserAuditLogins, this.GetConfigBasedCheckBox(this.chkUserAuditLoginsDatabase, this.chkUserAuditLogins));
            // SQLCM 5563: Disable Logout with Logins for Server settings not properly configured while applying Regulation Guideline at Server Level
            if (this._server.AuditLogins)
            {
                DeselectionManager.SetAdditiveCheckbox(this._server.AuditLogouts, this.chkUserAuditLogouts, _checkLogouts, this.GetConfigBasedCheckBox(this.chkUserAuditLogoutsDatabase, this.chkUserAuditLogouts));
            }

            DeselectionManager.SetAdditiveCheckbox(this._server.AuditSecurity, _checkSecurityChanges, _chkAuditSecurityChanges, this.chkUserAuditSecurity, this.GetConfigBasedCheckBox(this.chkUserAuditSecurityDatabase, this.chkUserAuditSecurity));

            DeselectionManager.SetAdditiveCheckbox(this._server.AuditUDE, _checkUserDefinedEvents, this.chkUserAuditUDE, this.GetConfigBasedCheckBox(this.chkUserAuditUDEDatabase, this.chkUserAuditUDE));

            if (_server.AuditAccessCheck == AccessCheckFilter.FailureOnly)
            {
                CheckRb(this.GetConfigBasedRadioButton(this.rbUserAuditFailedOnlyDatabase, this.rbUserAuditFailedOnly), true);
                _rbDBAuditFailedOnly.Checked = rbUserAuditFailedOnly.Checked = _rbAuditFailedOnly.Checked = true;

                CheckCb(this.GetConfigBasedCheckBox(this.chkUserAccessCheckFilterDatabase, this.chkUserAccessCheckFilter), true);
                _chkDBAccessCheckFilter.Checked = chkUserAccessCheckFilter.Checked = _chkServerAccessCheckFilter.Checked = true;

                EnableRb(this.GetConfigBasedRadioButton(this.rbUserAuditFailedOnlyDatabase, this.rbUserAuditFailedOnly), true);
                _rbDBAuditFailedOnly.Enabled = rbUserAuditFailedOnly.Enabled = _rbAuditFailedOnly.Enabled = true;

                EnableRb(this.GetConfigBasedRadioButton(this.rbUserAuditSuccessfulOnlyDatabase, this.rbUserAuditSuccessfulOnly), true);
                _rbDBAuditSuccessfulOnly.Enabled = rbUserAuditSuccessfulOnly.Enabled = _rbServerAuditSuccessfulOnly.Enabled = true;
            }
            else if (_server.AuditAccessCheck == AccessCheckFilter.SuccessOnly)
            {
                CheckRb(this.GetConfigBasedRadioButton(this.rbUserAuditSuccessfulOnlyDatabase, this.rbUserAuditSuccessfulOnly), true);
                _rbDBAuditSuccessfulOnly.Checked = rbUserAuditSuccessfulOnly.Checked = _rbServerAuditSuccessfulOnly.Checked = true;

                CheckCb(this.GetConfigBasedCheckBox(this.chkUserAccessCheckFilterDatabase, this.chkUserAccessCheckFilter), true);
                _chkDBAccessCheckFilter.Checked = chkUserAccessCheckFilter.Checked = _chkServerAccessCheckFilter.Checked = true;

                EnableRb(this.GetConfigBasedRadioButton(this.rbUserAuditFailedOnlyDatabase, this.rbUserAuditFailedOnly), true);
                _rbDBAuditFailedOnly.Enabled = rbUserAuditFailedOnly.Enabled = _rbAuditFailedOnly.Enabled = true;

                EnableRb(this.GetConfigBasedRadioButton(this.rbUserAuditSuccessfulOnlyDatabase, this.rbUserAuditSuccessfulOnly), true);
                _rbDBAuditSuccessfulOnly.Enabled = rbUserAuditSuccessfulOnly.Enabled = _rbServerAuditSuccessfulOnly.Enabled = true;
            }
            else
            {
                CheckCb(this.GetConfigBasedCheckBox(this.chkUserAccessCheckFilterDatabase, this.chkUserAccessCheckFilter), false);
                _chkDBAccessCheckFilter.Checked = chkUserAccessCheckFilter.Checked = _chkServerAccessCheckFilter.Checked = false;

                EnableRb(this.GetConfigBasedRadioButton(this.rbUserAuditFailedOnlyDatabase, this.rbUserAuditFailedOnly), false);
                _rbDBAuditFailedOnly.Enabled = rbUserAuditFailedOnly.Enabled = _rbAuditFailedOnly.Enabled = false;

                EnableRb(this.GetConfigBasedRadioButton(this.rbUserAuditSuccessfulOnlyDatabase, this.rbUserAuditSuccessfulOnly), false);
                _rbDBAuditSuccessfulOnly.Enabled = rbUserAuditSuccessfulOnly.Enabled = _rbServerAuditSuccessfulOnly.Enabled = false;
            }
        }

        private void CheckCb(CheckBox checkbox, bool checkedValue)
        {
            if (checkbox != null)
            {
                checkbox.Checked = checkedValue;
            }
        }

        private void EnableCb(CheckBox checkbox, bool checkedValue)
        {
            if (checkbox != null)
            {
                checkbox.Enabled = checkedValue;
            }
        }

        private void CheckRb(RadioButton radioButton, bool checkedValue)
        {
            if (radioButton != null)
            {
                radioButton.Checked = checkedValue;
            }
        }

        private void EnableRb(RadioButton radioButton, bool enabledValue)
        {
            if (radioButton != null)
            {
                radioButton.Enabled = enabledValue;
            }
        }

        private void InitializeServerPrivilegeUsersUi()
        {
            if (_templateSvrInitializationCompleted)
                return;
            DeselectionManager.SetAdditiveCheckbox(this._server.AuditUserAdmin, this.chkUserAuditAdmin, this.GetConfigBasedCheckBox(this.chkUserAuditAdminDatabase, this.chkUserAuditAdmin));
            DeselectionManager.SetAdditiveCheckbox(this._server.AuditUserCaptureSQL, this.chkUserAuditCaptureSQL, this.GetConfigBasedCheckBox(this.chkUserAuditCaptureSQLDatabase, this.chkUserAuditCaptureSQL));
            DeselectionManager.SetAdditiveCheckbox(this._server.AuditUserCaptureDDL, this.chkUserCaptureDDL, this.GetConfigBasedCheckBox(this.chkUserCaptureDDLDatabase, this.chkUserCaptureDDL));
            DeselectionManager.SetAdditiveCheckbox(this._server.AuditUserCaptureTrans, this.chkUserAuditCaptureTrans, this.GetConfigBasedCheckBox(this.chkUserAuditCaptureTransDatabase, this.chkUserAuditCaptureTrans));
            DeselectionManager.SetAdditiveCheckbox(this._server.AuditUserDDL, this.chkUserAuditDDL, this.GetConfigBasedCheckBox(this.chkUserAuditDDLDatabase, this.chkUserAuditDDL));
            DeselectionManager.SetAdditiveCheckbox(this._server.AuditUserDML, this.chkUserAuditDML, this.GetConfigBasedCheckBox(this.chkUserAuditDMLDatabase, this.chkUserAuditDML));
            DeselectionManager.SetAdditiveCheckbox(this._server.AuditUserFailedLogins, this.chkUserAuditFailedLogins, this.GetConfigBasedCheckBox(this.chkUserAuditFailedLoginsDatabase, this.chkUserAuditFailedLogins));
            DeselectionManager.SetAdditiveCheckbox(this._server.AuditUserLogins, this.chkUserAuditLogins, this.GetConfigBasedCheckBox(this.chkUserAuditLoginsDatabase, this.chkUserAuditLogins));
            // SQLCM 5563: Disable Logout with Logins for Server settings not properly configured while applying Regulation Guideline at Server Level
            if (_server.AuditUserLogins)
            {
                DeselectionManager.SetAdditiveCheckbox(this._server.AuditUserLogouts, this.chkUserAuditLogouts, this.GetConfigBasedCheckBox(this.chkUserAuditLogoutsDatabase, this.chkUserAuditLogouts));
            }
            DeselectionManager.SetAdditiveCheckbox(this._server.AuditUserSecurity, this.chkUserAuditSecurity, this.GetConfigBasedCheckBox(this.chkUserAuditSecurityDatabase, this.chkUserAuditSecurity));
            DeselectionManager.SetAdditiveCheckbox(this._server.AuditUserSELECT, this.chkUserAuditSELECT, this.GetConfigBasedCheckBox(this.chkUserAuditSELECTDatabase, this.chkUserAuditSELECT));
            DeselectionManager.SetAdditiveCheckbox(this._server.AuditUserUDE, this.chkUserAuditUDE, this.GetConfigBasedCheckBox(this.chkUserAuditUDEDatabase, this.chkUserAuditUDE));

            if (_server.AuditAccessCheck == AccessCheckFilter.NoFilter)
            {
                if (_server.AuditUserAccessCheck == AccessCheckFilter.FailureOnly)
                {
                    CheckRb(this.GetConfigBasedRadioButton(this.rbUserAuditFailedOnlyDatabase, this.rbUserAuditFailedOnly), true);
                    CheckCb(this.GetConfigBasedCheckBox(this.chkUserAccessCheckFilterDatabase, this.chkUserAccessCheckFilter), true);
                    EnableRb(this.GetConfigBasedRadioButton(this.rbUserAuditFailedOnlyDatabase, this.rbUserAuditFailedOnly), true);
                    EnableRb(this.GetConfigBasedRadioButton(this.rbUserAuditSuccessfulOnlyDatabase, this.rbUserAuditSuccessfulOnly), true);

                    rbUserAuditFailedOnly.Checked = true;
                    chkUserAccessCheckFilter.Checked = true;
                    rbUserAuditFailedOnly.Enabled = true;
                    rbUserAuditSuccessfulOnly.Enabled = true;
                }
                else if (_server.AuditUserAccessCheck == AccessCheckFilter.SuccessOnly)
                {
                    CheckRb(this.GetConfigBasedRadioButton(this.rbUserAuditSuccessfulOnlyDatabase, this.rbUserAuditSuccessfulOnly), true);
                    CheckCb(this.GetConfigBasedCheckBox(this.chkUserAccessCheckFilterDatabase, this.chkUserAccessCheckFilter), true);
                    EnableRb(this.GetConfigBasedRadioButton(this.rbUserAuditFailedOnlyDatabase, this.rbUserAuditFailedOnly), true);
                    EnableRb(this.GetConfigBasedRadioButton(this.rbUserAuditSuccessfulOnlyDatabase, this.rbUserAuditSuccessfulOnly), true);

                    rbUserAuditSuccessfulOnly.Checked = true;
                    chkUserAccessCheckFilter.Checked = true;
                    rbUserAuditFailedOnly.Enabled = true;
                    rbUserAuditSuccessfulOnly.Enabled = true;
                }
                else
                {
                    CheckCb(this.GetConfigBasedCheckBox(this.chkUserAccessCheckFilterDatabase, this.chkUserAccessCheckFilter), false);
                    EnableRb(this.GetConfigBasedRadioButton(this.rbUserAuditFailedOnlyDatabase, this.rbUserAuditFailedOnly), false);
                    EnableRb(this.GetConfigBasedRadioButton(this.rbUserAuditSuccessfulOnlyDatabase, this.rbUserAuditSuccessfulOnly), false);
                    chkUserAccessCheckFilter.Checked = false;
                    rbUserAuditFailedOnly.Enabled = false;
                    rbUserAuditSuccessfulOnly.Enabled = false;
                }
            }
        }

        private void InitializeDatabasePrivilegeUsersUi()
        {
            if (_templateSvrInitializationCompleted)
                return;
            if (_databases != null && _databases.Count > 0)
            {
                var dbRecord = _databases[0];

                CheckCb(this.GetConfigBasedCheckBox(this.chkUserAuditAdminDatabase, this.chkUserAuditAdmin), dbRecord.AuditUserAdmin);
                CheckCb(this.GetConfigBasedCheckBox(this.chkUserAuditCaptureSQLDatabase, this.chkUserAuditCaptureSQL), dbRecord.AuditUserCaptureSQL);
                CheckCb(this.GetConfigBasedCheckBox(this.chkUserCaptureDDLDatabase, this.chkUserCaptureDDL), dbRecord.AuditUserCaptureDDL);
                CheckCb(this.GetConfigBasedCheckBox(this.chkUserAuditCaptureTransDatabase, this.chkUserAuditCaptureTrans), dbRecord.AuditUserCaptureTrans);
                CheckCb(this.GetConfigBasedCheckBox(this.chkUserAuditDDLDatabase, this.chkUserAuditDDL), dbRecord.AuditUserDDL);
                CheckCb(this.GetConfigBasedCheckBox(this.chkUserAuditDMLDatabase, this.chkUserAuditDML), dbRecord.AuditUserDML);
                CheckCb(this.GetConfigBasedCheckBox(this.chkUserAuditFailedLoginsDatabase, this.chkUserAuditFailedLogins), dbRecord.AuditUserFailedLogins);
                CheckCb(this.GetConfigBasedCheckBox(this.chkUserAuditLoginsDatabase, this.chkUserAuditLogins), dbRecord.AuditUserLogins);
                // SQLCM 5563: Disable Logout with Logins for Server settings not properly configured while applying Regulation Guideline at Server Level
                if (dbRecord.AuditUserLogins)
                {
                    CheckCb(this.GetConfigBasedCheckBox(this.chkUserAuditLogoutsDatabase, this.chkUserAuditLogouts), dbRecord.AuditUserLogouts);
                }
                CheckCb(this.GetConfigBasedCheckBox(this.chkUserAuditSecurityDatabase, this.chkUserAuditSecurity), dbRecord.AuditUserSecurity);
                CheckCb(this.GetConfigBasedCheckBox(this.chkUserAuditSELECTDatabase, this.chkUserAuditSELECT), dbRecord.AuditUserSELECT);
                CheckCb(this.GetConfigBasedCheckBox(this.chkUserAuditUDEDatabase, this.chkUserAuditUDE), dbRecord.AuditUserUDE);

                if (this._server.AuditAccessCheck == AccessCheckFilter.NoFilter
                    && this._server.AuditUserAccessCheck == AccessCheckFilter.NoFilter && dbRecord.AuditAccessCheck == AccessCheckFilter.NoFilter)
                {
                    if (dbRecord.AuditUserAccessCheck == AccessCheckFilter.FailureOnly)
                    {
                        CheckRb(this.GetConfigBasedRadioButton(this.rbUserAuditFailedOnlyDatabase, this.rbUserAuditFailedOnly), true);
                        CheckCb(this.GetConfigBasedCheckBox(this.chkUserAccessCheckFilterDatabase, this.chkUserAccessCheckFilter), true);
                        EnableRb(this.GetConfigBasedRadioButton(this.rbUserAuditFailedOnlyDatabase, this.rbUserAuditFailedOnly), true);
                        EnableRb(this.GetConfigBasedRadioButton(this.rbUserAuditSuccessfulOnlyDatabase, this.rbUserAuditSuccessfulOnly), true);
                    }
                    else if (dbRecord.AuditUserAccessCheck == AccessCheckFilter.SuccessOnly)
                    {
                        CheckRb(this.GetConfigBasedRadioButton(this.rbUserAuditSuccessfulOnlyDatabase, this.rbUserAuditSuccessfulOnly), true);
                        CheckCb(this.GetConfigBasedCheckBox(this.chkUserAccessCheckFilterDatabase, this.chkUserAccessCheckFilter), true);
                        EnableRb(this.GetConfigBasedRadioButton(this.rbUserAuditFailedOnlyDatabase, this.rbUserAuditFailedOnly), true);
                        EnableRb(this.GetConfigBasedRadioButton(this.rbUserAuditSuccessfulOnlyDatabase, this.rbUserAuditSuccessfulOnly), true);
                    }
                    else
                    {
                        CheckCb(this.GetConfigBasedCheckBox(this.chkUserAccessCheckFilterDatabase, this.chkUserAccessCheckFilter), false);
                        EnableRb(this.GetConfigBasedRadioButton(this.rbUserAuditFailedOnlyDatabase, this.rbUserAuditFailedOnly), false);
                        EnableRb(this.GetConfigBasedRadioButton(this.rbUserAuditSuccessfulOnlyDatabase, this.rbUserAuditSuccessfulOnly), false);
                    }
                }
            }
        }

        private void InitializeServerAuditDeselection()
        {
            if (_templateSvrInitializationCompleted)
                return;

            this.InitializeServerDeselectionUi();

            if (Globals.isServerNodeSelected)
            {
                // Register the Server Privilege Users Dependencies
                _deselectionManager.RegisterCheckbox(_checkLogins, DeselectControls.ServerLogins);
                _deselectionManager.RegisterCheckbox(_checkLogouts, DeselectControls.ServerLogouts);
                _deselectionManager.RegisterCheckbox(_checkFailedLogins, DeselectControls.ServerFailedLogins);
                _deselectionManager.RegisterCheckbox(_checkSecurityChanges, DeselectControls.ServerSecurityChanges);
                _deselectionManager.RegisterCheckbox(_checkDDL, DeselectControls.ServerDatabaseDefinition);
                _deselectionManager.RegisterCheckbox(_checkAdministrativeActivities, DeselectControls.ServerAdministrativeActivities);
                _deselectionManager.RegisterCheckbox(_chkServerAccessCheckFilter, DeselectControls.ServerFilterEvents);
                _deselectionManager.RegisterRadioButton(_rbServerAuditSuccessfulOnly, DeselectControls.ServerFilterEventsPassOnly);
                _deselectionManager.RegisterRadioButton(_rbAuditFailedOnly, DeselectControls.ServerFilterEventsFailedOnly);
                _deselectionManager.RegisterCheckbox(_checkUserDefinedEvents, DeselectControls.ServerUserDefined);

                _deselectionManager.RegisterCheckbox(chkUserAuditLogins, DeselectControls.ServerUserLogins);
                _deselectionManager.RegisterCheckbox(chkUserAuditLogouts, DeselectControls.ServerUserLogouts);
                _deselectionManager.RegisterCheckbox(chkUserAuditFailedLogins, DeselectControls.ServerUserFailedLogins);
                _deselectionManager.RegisterCheckbox(chkUserAuditSecurity, DeselectControls.ServerUserSecurityChanges);
                _deselectionManager.RegisterCheckbox(chkUserAuditDDL, DeselectControls.ServerUserDatabaseDefinition);
                _deselectionManager.RegisterCheckbox(chkUserAuditAdmin, DeselectControls.ServerUserAdministrativeActivities);
                _deselectionManager.RegisterCheckbox(chkUserAuditSELECT, DeselectControls.ServerUserDatabaseSelect);
                _deselectionManager.RegisterCheckbox(chkUserAuditDML, DeselectControls.ServerUserDatabaseModifications);
                _deselectionManager.RegisterCheckbox(chkUserAuditCaptureSQL, DeselectControls.ServerUserCaptureSqlDmlSelect);
                _deselectionManager.RegisterCheckbox(chkUserAuditCaptureTrans, DeselectControls.ServerUserCaptureSqlTransactionStatus);
                _deselectionManager.RegisterCheckbox(chkUserAccessCheckFilter, DeselectControls.ServerUserFilterEvents);
                _deselectionManager.RegisterRadioButton(rbUserAuditSuccessfulOnly, DeselectControls.ServerUserFilterEventsPassOnly);
                _deselectionManager.RegisterRadioButton(rbUserAuditFailedOnly, DeselectControls.ServerUserFilterEventsFailedOnly);
                _deselectionManager.RegisterCheckbox(chkUserAuditUDE, DeselectControls.ServerUserUde);
                _deselectionManager.RegisterCheckbox(chkUserCaptureDDL, DeselectControls.ServerUserCaptureSqlDdlSecurity);
            }

        }

        /// <summary>
        /// Initialize Server Deselection UI
        /// </summary>
        private void InitializeServerDeselectionUi()
        {
            // Add for Database Privilege Users Settings after pulling latest
            DeselectionManager.GreyOutCheckboxControls(_server.AuditLogins, _checkLogins, this.chkUserAuditLogins, this.GetConfigBasedCheckBox(this.chkUserAuditLoginsDatabase, this.chkUserAuditLogins));
            // SQLCM 5563: Disable Logout with Logins for Server settings not properly configured while applying Regulation Guideline at Server Level
            if (_server.AuditLogins)
            {
                DeselectionManager.GreyOutCheckboxControls(_server.AuditLogouts, _checkLogouts, this.chkUserAuditLogouts, this.GetConfigBasedCheckBox(this.chkUserAuditLogoutsDatabase, this.chkUserAuditLogouts));
            }
            DeselectionManager.GreyOutCheckboxControls(_server.AuditFailedLogins, _checkFailedLogins, this.chkUserAuditFailedLogins, this.GetConfigBasedCheckBox(this.chkUserAuditFailedLoginsDatabase, this.chkUserAuditFailedLogins));
            DeselectionManager.GreyOutCheckboxControls(_server.AuditSecurity, _checkSecurityChanges, _chkAuditSecurityChanges, this.chkUserAuditSecurity, this.GetConfigBasedCheckBox(this.chkUserAuditSecurityDatabase, this.chkUserAuditSecurity));
            DeselectionManager.GreyOutCheckboxControls(_server.AuditDDL, _chkAuditDDL, this.chkUserAuditDDL, _checkDDL, this.GetConfigBasedCheckBox(this.chkUserAuditDDLDatabase, this.chkUserAuditDDL));
            DeselectionManager.GreyOutCheckboxControls(_server.AuditAdmin, _chkAuditAdminActivity, this.chkUserAuditAdmin, _checkAdministrativeActivities, this.GetConfigBasedCheckBox(this.chkUserAuditAdminDatabase, this.chkUserAuditAdmin));
            DeselectionManager.GreyOutCheckboxControls(_server.AuditDML, _chkAuditDML, this.chkUserAuditDML, this.GetConfigBasedCheckBox(this.chkUserAuditDMLDatabase, this.chkUserAuditDML));
            DeselectionManager.GreyOutCheckboxControls(_server.AuditUDE, this.chkUserAuditUDE, _checkUserDefinedEvents, this.GetConfigBasedCheckBox(this.chkUserAuditUDEDatabase, this.chkUserAuditUDE));
            switch (_server.AuditAccessCheck)
            {
                case AccessCheckFilter.NoFilter:
                    break;
                case AccessCheckFilter.SuccessOnly:
                    DeselectionManager.GreyOutCheckboxControls(true, _chkServerAccessCheckFilter, this.chkUserAccessCheckFilter, _chkDBAccessCheckFilter, this.GetConfigBasedCheckBox(this.chkUserAccessCheckFilterDatabase, this.chkUserAccessCheckFilter));
                    DeselectionManager.GreyOutRadioButtonControls(true, this.rbUserAuditSuccessfulOnly, _rbServerAuditSuccessfulOnly, _rbDBAuditSuccessfulOnly, this.GetConfigBasedRadioButton(this.rbUserAuditSuccessfulOnlyDatabase, this.rbUserAuditSuccessfulOnly));
                    DeselectionManager.DisableRadioButtonControls(true, this.rbUserAuditFailedOnly, _rbAuditFailedOnly, _rbDBAuditFailedOnly, this.GetConfigBasedRadioButton(this.rbUserAuditFailedOnlyDatabase, this.rbUserAuditFailedOnly));
                    break;
                case AccessCheckFilter.FailureOnly:
                    DeselectionManager.GreyOutCheckboxControls(true, _chkServerAccessCheckFilter, this.chkUserAccessCheckFilter, _chkDBAccessCheckFilter, this.GetConfigBasedCheckBox(this.chkUserAccessCheckFilterDatabase, this.chkUserAccessCheckFilter));
                    DeselectionManager.DisableRadioButtonControls(true, this.rbUserAuditSuccessfulOnly, _rbServerAuditSuccessfulOnly, _rbDBAuditSuccessfulOnly, this.GetConfigBasedRadioButton(this.rbUserAuditSuccessfulOnlyDatabase, this.rbUserAuditSuccessfulOnly));
                    DeselectionManager.GreyOutRadioButtonControls(true, this.rbUserAuditFailedOnly, _rbAuditFailedOnly, _rbDBAuditFailedOnly, this.GetConfigBasedRadioButton(this.rbUserAuditFailedOnlyDatabase, this.rbUserAuditFailedOnly));
                    break;
            }

            DeselectionManager.GreyOutCheckboxControls(_server.AuditUserLogins, this.chkUserAuditLogins, this.GetConfigBasedCheckBox(this.chkUserAuditLoginsDatabase, this.chkUserAuditLogins));
            // SQLCM 5563: Disable Logout with Logins for Server settings not properly configured while applying Regulation Guideline at Server Level
            if (_server.AuditUserLogins)
            {
                DeselectionManager.GreyOutCheckboxControls(_server.AuditUserLogouts, this.chkUserAuditLogouts, this.GetConfigBasedCheckBox(this.chkUserAuditLogoutsDatabase, this.chkUserAuditLogouts));
            }
            DeselectionManager.GreyOutCheckboxControls(_server.AuditUserFailedLogins, this.chkUserAuditFailedLogins, this.GetConfigBasedCheckBox(this.chkUserAuditFailedLoginsDatabase, this.chkUserAuditFailedLogins));
            DeselectionManager.GreyOutCheckboxControls(_server.AuditUserSecurity, this.chkUserAuditSecurity, this.GetConfigBasedCheckBox(this.chkUserAuditSecurityDatabase, this.chkUserAuditSecurity));
            DeselectionManager.GreyOutCheckboxControls(_server.AuditUserDDL, this.chkUserAuditDDL, this.GetConfigBasedCheckBox(this.chkUserAuditDDLDatabase, this.chkUserAuditDDL));
            DeselectionManager.GreyOutCheckboxControls(_server.AuditUserAdmin, this.chkUserAuditAdmin, this.GetConfigBasedCheckBox(this.chkUserAuditAdminDatabase, this.chkUserAuditAdmin));
            DeselectionManager.GreyOutCheckboxControls(_server.AuditUserCaptureSQL, this.chkUserAuditCaptureSQL, this.GetConfigBasedCheckBox(this.chkUserAuditCaptureSQLDatabase, this.chkUserAuditCaptureSQL));
            DeselectionManager.GreyOutCheckboxControls(_server.AuditUserCaptureDDL, this.chkUserCaptureDDL, this.GetConfigBasedCheckBox(this.chkUserCaptureDDLDatabase, this.chkUserCaptureDDL));
            DeselectionManager.GreyOutCheckboxControls(_server.AuditUserCaptureTrans, this.chkUserAuditCaptureTrans, this.GetConfigBasedCheckBox(this.chkUserAuditCaptureTransDatabase, this.chkUserAuditCaptureTrans));
            DeselectionManager.GreyOutCheckboxControls(_server.AuditUserSELECT, this.chkUserAuditSELECT, this.GetConfigBasedCheckBox(this.chkUserAuditSELECTDatabase, this.chkUserAuditSELECT));
            DeselectionManager.GreyOutCheckboxControls(_server.AuditUserDML, this.chkUserAuditDML, this.GetConfigBasedCheckBox(this.chkUserAuditDMLDatabase, this.chkUserAuditDML));
            DeselectionManager.GreyOutCheckboxControls(_server.AuditUserUDE, this.chkUserAuditUDE, this.GetConfigBasedCheckBox(this.chkUserAuditUDEDatabase, this.chkUserAuditUDE));
            if (_server.AuditAccessCheck == AccessCheckFilter.NoFilter)
            {
                switch (_server.AuditUserAccessCheck)
                {
                    case AccessCheckFilter.NoFilter: break;
                    case AccessCheckFilter.SuccessOnly:
                        DeselectionManager.GreyOutCheckboxControls(true, this.chkUserAccessCheckFilter, this.GetConfigBasedCheckBox(this.chkUserAccessCheckFilterDatabase, this.chkUserAccessCheckFilter));
                        DeselectionManager.GreyOutRadioButtonControls(true, this.rbUserAuditSuccessfulOnly, this.GetConfigBasedRadioButton(this.rbUserAuditSuccessfulOnlyDatabase, this.rbUserAuditSuccessfulOnly));
                        DeselectionManager.DisableRadioButtonControls(true, this.rbUserAuditFailedOnly, this.GetConfigBasedRadioButton(this.rbUserAuditFailedOnlyDatabase, this.rbUserAuditFailedOnly));
                        break;
                    case AccessCheckFilter.FailureOnly:
                        DeselectionManager.GreyOutCheckboxControls(true, this.chkUserAccessCheckFilter, this.GetConfigBasedCheckBox(this.chkUserAccessCheckFilterDatabase, this.chkUserAccessCheckFilter));
                        DeselectionManager.DisableRadioButtonControls(true, this.rbUserAuditSuccessfulOnly, this.GetConfigBasedRadioButton(this.rbUserAuditSuccessfulOnlyDatabase, this.rbUserAuditSuccessfulOnly));
                        DeselectionManager.GreyOutRadioButtonControls(true, this.rbUserAuditFailedOnly, this.GetConfigBasedRadioButton(this.rbUserAuditFailedOnlyDatabase, this.rbUserAuditFailedOnly));
                        break;
                }
            }

        }

        /// <summary>
        /// Deselection Control
        /// </summary>
        /// <param name="currentControlChecked"></param>
        /// <param name="deselectValue"></param>
        public void UpdateUiControls(bool currentControlChecked, DeselectValues deselectValue)
        {
            // Server Privilege Level Pending Properties
            var dbRecord = this._databases != null && _databases.Count > 0 ? this._databases[0] : null;
            var dependentServerRegulation = Globals.isServerNodeSelected
                                       && _startPage == StartPage.RegulationGuidelineInfo;
            var deselectOption = deselectValue.DeselectOption;
            var deselectControl = deselectValue.DeselectControl;
            // SQLCM 5563: Disable Logout with Logins for Server settings not properly configured while applying Regulation Guideline at Server Level
            var preventDependentRegulation = (!dependentServerRegulation || currentControlChecked
                                             || deselectOption == DeselectOptions.OtherLevels) && !_isNewServer;
            // perform action on deselect options and property
            switch (deselectControl)
            {
                case DeselectControls.ServerLogins:
                    this.UpdateDependentCheckboxes(
                        currentControlChecked,
                        deselectOption,
                        deselectControl,
                        null,
                        this.chkUserAuditLogins,
                        (preventDependentRegulation || !this.chkUserAuditLogins.Checked)
                            ? this.GetConfigBasedCheckBox(this.chkUserAuditLoginsDatabase, this.chkUserAuditLogins)
                            : null);
                    // SQLCM 5563: Disable Logout with Logins for Server settings not properly configured while applying Regulation Guideline at Server Level
                    if (!currentControlChecked)
                    {
                        switch (deselectOption)
                        {
                            case DeselectOptions.None: break;
                            case DeselectOptions.CurrentLevelOnly:
                                this.UpdateDependentCheckboxes(
                                    currentControlChecked,
                                    deselectOption,
                                    deselectControl,
                                    null,
                                    this.chkUserAuditLogouts,
                                    (preventDependentRegulation || !this.chkUserAuditLogins.Checked)
                                        ? this.GetConfigBasedCheckBox(this.chkUserAuditLogoutsDatabase, this.chkUserAuditLogouts)
                                        : null);
                                break;
                            case DeselectOptions.OtherLevels:
                                EnableCb(this.GetConfigBasedCheckBox(this.chkUserAuditLogoutsDatabase, this.chkUserAuditLogouts), false);
                                this.chkUserAuditLogouts.Enabled = false;
                                CheckCb(this.GetConfigBasedCheckBox(this.chkUserAuditLogoutsDatabase, this.chkUserAuditLogouts), false);
                                this.chkUserAuditLogouts.Checked = false;
                                break;
                        }
                    }
                    break;
                case DeselectControls.ServerLogouts:
                    this.UpdateDependentCheckboxes(
                        currentControlChecked,
                        deselectOption,
                        deselectControl,
                        null,
                        this.chkUserAuditLogouts,
                        (preventDependentRegulation || !this.chkUserAuditLogouts.Checked)
                            ? this.GetConfigBasedCheckBox(this.chkUserAuditLogoutsDatabase, this.chkUserAuditLogouts)
                            : null);
                    break;
                case DeselectControls.ServerFailedLogins:
                    this.UpdateDependentCheckboxes(
                        currentControlChecked,
                        deselectOption,
                        deselectControl,
                        null,
                        this.chkUserAuditFailedLogins,
                        (preventDependentRegulation || !this.chkUserAuditFailedLogins.Checked)
                            ? this.GetConfigBasedCheckBox(this.chkUserAuditFailedLoginsDatabase, this.chkUserAuditFailedLogins)
                            : null);
                    break;
                case DeselectControls.ServerSecurityChanges:
                    this.UpdateDependentCheckboxes(
                        currentControlChecked,
                        deselectOption,
                        deselectControl,
                        null,
                        this.chkUserAuditSecurity,
                        this._chkAuditSecurityChanges,
                        (preventDependentRegulation || !this.chkUserAuditSecurity.Checked
                         || !this.chkDBAuditSecurity.Checked)
                            ? this.GetConfigBasedCheckBox(this.chkUserAuditSecurityDatabase, this.chkUserAuditSecurity)
                            : null,
                        this.chkDBAuditSecurity);
                    break;
                case DeselectControls.ServerDatabaseDefinition:
                    this.UpdateDependentCheckboxes(
                        currentControlChecked,
                        deselectOption,
                        deselectControl,
                        null,
                        this.chkUserAuditDDL,
                        this._chkAuditDDL,
                        (preventDependentRegulation || !this.chkUserAuditDDL.Checked
                         || !this.chkDBAuditDDL.Checked)
                            ? this.GetConfigBasedCheckBox(this.chkUserAuditDDLDatabase, this.chkUserAuditDDL)
                            : null,
                        this.chkDBAuditDDL);
                    break;
                case DeselectControls.ServerAdministrativeActivities:
                    this.UpdateDependentCheckboxes(
                        currentControlChecked,
                        deselectOption,
                        deselectControl,
                        null,
                        this.chkUserAuditAdmin,
                        this._chkAuditAdminActivity,
                        (preventDependentRegulation || !this.chkUserAuditAdmin.Checked || !this.chkDBAuditAdmin.Checked)
                            ? this.GetConfigBasedCheckBox(this.chkUserAuditAdminDatabase, this.chkUserAuditAdmin)
                            : null,
                        this.chkDBAuditAdmin);
                    break;
                case DeselectControls.ServerFilterEvents:
                    this.UpdateDependentCheckboxes(
                        currentControlChecked,
                        deselectOption,
                        deselectControl,
                        null,
                        this.chkUserAccessCheckFilter,
                        _chkDBAccessCheckFilter,
                        (preventDependentRegulation || !this.chkUserAccessCheckFilter.Checked || !_chkDBAccessCheckFilter.Checked)
                            ? this.GetConfigBasedCheckBox(this.chkUserAccessCheckFilterDatabase, this.chkUserAccessCheckFilter)
                            : null,
                        chkDBAccessCheckFilter);
                    if (currentControlChecked)
                    {
                        // For Unchecked Values we don't have to set the radio buttons
                        this.UpdateDependentRadioButtons(
                            preventDependentRegulation ? this._rbServerAuditSuccessfulOnly.Checked : rbServerAuditSuccessfulOnly.Checked,
                            deselectOption,
                            deselectControl,
                            null,
                            this.rbUserAuditSuccessfulOnly,
                            _rbDBAuditSuccessfulOnly,
                            this.GetConfigBasedRadioButton(
                                this.rbUserAuditSuccessfulOnlyDatabase,
                                this.rbUserAuditSuccessfulOnly),
                            rbDBAuditSuccessfulOnly);
                        this.UpdateDependentRadioButtons(
                            preventDependentRegulation ? this._rbAuditFailedOnly.Checked : rbAuditFailedOnly.Checked,
                            deselectOption,
                            deselectControl,
                            null,
                            this.rbUserAuditFailedOnly,
                            _rbDBAuditFailedOnly,
                            this.GetConfigBasedRadioButton(this.rbUserAuditFailedOnlyDatabase, this.rbUserAuditFailedOnly),
                            rbDBAuditFailedOnly);
                    }
                    else
                    {
                        switch (deselectOption)
                        {
                            case DeselectOptions.None:
                                break;
                            case DeselectOptions.CurrentLevelOnly:
                                if (preventDependentRegulation || !this.chkUserAccessCheckFilter.Checked
                                    || !this._chkDBAccessCheckFilter.Checked)
                                {
                                    this.CheckCb(
                                        this.GetConfigBasedCheckBox(
                                            this.chkUserAccessCheckFilterDatabase,
                                            this.chkUserAccessCheckFilter),
                                        false);
                                    EnableCb(
                                        this.GetConfigBasedCheckBox(
                                            this.chkUserAccessCheckFilterDatabase,
                                            this.chkUserAccessCheckFilter),
                                        true);
                                    EnableRb(
                                        this.GetConfigBasedRadioButton(
                                            this.rbUserAuditFailedOnlyDatabase,
                                            this.rbUserAuditFailedOnly),
                                        false);
                                    EnableRb(
                                        this.GetConfigBasedRadioButton(
                                            this.rbUserAuditSuccessfulOnlyDatabase,
                                            this.rbUserAuditSuccessfulOnly),
                                        false);
                                }

                                chkUserAccessCheckFilter.Enabled = true;
                                rbUserAuditFailedOnly.Enabled = rbUserAuditSuccessfulOnly.Enabled = chkUserAccessCheckFilter.Checked;

                                chkDBAccessCheckFilter.Enabled = true;
                                rbDBAuditFailedOnly.Enabled = rbDBAuditSuccessfulOnly.Enabled = chkDBAccessCheckFilter.Checked;

                                _chkDBAccessCheckFilter.Enabled = true;
                                _rbDBAuditFailedOnly.Enabled = _rbDBAuditSuccessfulOnly.Enabled = _chkDBAccessCheckFilter.Checked;

                                break;
                            case DeselectOptions.OtherLevels:
                                this.CheckCb(
                                    this.GetConfigBasedCheckBox(
                                        this.chkUserAccessCheckFilterDatabase,
                                        this.chkUserAccessCheckFilter),
                                    false);
                                EnableCb(
                                    this.GetConfigBasedCheckBox(
                                        this.chkUserAccessCheckFilterDatabase,
                                        this.chkUserAccessCheckFilter),
                                    true);
                                EnableRb(
                                    this.GetConfigBasedRadioButton(
                                        this.rbUserAuditFailedOnlyDatabase,
                                        this.rbUserAuditFailedOnly),
                                    false);
                                EnableRb(
                                    this.GetConfigBasedRadioButton(
                                        this.rbUserAuditSuccessfulOnlyDatabase,
                                        this.rbUserAuditSuccessfulOnly),
                                    false);

                                chkUserAccessCheckFilter.Checked = false;
                                chkUserAccessCheckFilter.Enabled = true;
                                rbUserAuditFailedOnly.Enabled = false;
                                rbUserAuditSuccessfulOnly.Enabled = false;

                                chkDBAccessCheckFilter.Checked = false;
                                chkDBAccessCheckFilter.Enabled = true;
                                rbDBAuditFailedOnly.Enabled = false;
                                rbDBAuditSuccessfulOnly.Enabled = false;

                                _chkDBAccessCheckFilter.Checked = false;
                                _chkDBAccessCheckFilter.Enabled = true;
                                _rbDBAuditFailedOnly.Enabled = false;
                                _rbDBAuditSuccessfulOnly.Enabled = false;
                                break;
                        }
                    }
                    break;
                case DeselectControls.ServerFilterEventsPassOnly:
                    // For Unchecked Values we don't have to set the radio buttons
                    this.UpdateDependentRadioButtons(preventDependentRegulation ? this._rbServerAuditSuccessfulOnly.Checked : rbServerAuditSuccessfulOnly.Checked, DeselectOptions.None, deselectControl, null, this.rbUserAuditSuccessfulOnly, _rbDBAuditSuccessfulOnly, this.GetConfigBasedRadioButton(this.rbUserAuditSuccessfulOnlyDatabase, this.rbUserAuditSuccessfulOnly), rbDBAuditSuccessfulOnly);
                    this.UpdateDependentRadioButtons(preventDependentRegulation ? this._rbAuditFailedOnly.Checked : rbAuditFailedOnly.Checked, DeselectOptions.None, deselectControl, null, this.rbUserAuditFailedOnly, _rbDBAuditFailedOnly, this.GetConfigBasedRadioButton(this.rbUserAuditFailedOnlyDatabase, this.rbUserAuditFailedOnly), rbDBAuditFailedOnly);
                    break;
                case DeselectControls.ServerFilterEventsFailedOnly:
                    // For Unchecked Values we don't have to set the radio buttons
                    this.UpdateDependentRadioButtons(preventDependentRegulation ? this._rbServerAuditSuccessfulOnly.Checked : rbServerAuditSuccessfulOnly.Checked, DeselectOptions.None, deselectControl, null, this.rbUserAuditSuccessfulOnly, _rbDBAuditSuccessfulOnly, this.GetConfigBasedRadioButton(this.rbUserAuditSuccessfulOnlyDatabase, this.rbUserAuditSuccessfulOnly), rbDBAuditSuccessfulOnly);
                    this.UpdateDependentRadioButtons(preventDependentRegulation ? this._rbAuditFailedOnly.Checked : rbAuditFailedOnly.Checked, DeselectOptions.None, deselectControl, null, this.rbUserAuditFailedOnly, _rbDBAuditFailedOnly, this.GetConfigBasedRadioButton(this.rbUserAuditFailedOnlyDatabase, this.rbUserAuditFailedOnly), rbDBAuditFailedOnly);
                    break;
                case DeselectControls.ServerUserDefined:
                    this.UpdateDependentCheckboxes(
                        currentControlChecked,
                        deselectOption,
                        deselectControl,
                        null,
                        this.chkUserAuditUDE,
                        this.GetConfigBasedCheckBox(this.chkUserAuditUDEDatabase, this.chkUserAuditUDE));
                    break;
                case DeselectControls.DbDatabaseDefinition:
                    this.UpdateDependentCheckboxes(
                        currentControlChecked,
                        deselectOption,
                        deselectControl,
                        null,
                        currentControlChecked || (Globals.isServerNodeSelected ? !chkUserAuditDDL.Checked : !_server.AuditUserDDL) ? this.GetConfigBasedCheckBox(this.chkUserAuditDDLDatabase, this.chkUserAuditDDL) : null);
                    break;
                case DeselectControls.DbSecurityChanges:
                    this.UpdateDependentCheckboxes(
                        currentControlChecked,
                        deselectOption,
                        deselectControl,
                        null,
                        currentControlChecked || (Globals.isServerNodeSelected ? !chkUserAuditSecurity.Checked : !_server.AuditUserSecurity) ? this.GetConfigBasedCheckBox(this.chkUserAuditSecurityDatabase, this.chkUserAuditSecurity) : null);
                    break;
                case DeselectControls.DbAdministrativeActivities:
                    this.UpdateDependentCheckboxes(
                        currentControlChecked,
                        deselectOption,
                        deselectControl,
                        null,
                        currentControlChecked || (Globals.isServerNodeSelected ? !chkUserAuditAdmin.Checked : !_server.AuditUserAdmin)
                            ? this.GetConfigBasedCheckBox(this.chkUserAuditSecurityDatabase, this.chkUserAuditSecurity)
                            : null);
                    break;
                case DeselectControls.DbDatabaseModifications:
                    this.UpdateDependentCheckboxes(
                        currentControlChecked,
                        deselectOption,
                        deselectControl,
                        null,
                        currentControlChecked || (Globals.isServerNodeSelected ? !chkUserAuditDML.Checked : !_server.AuditUserDML)
                            ? this.GetConfigBasedCheckBox(this.chkUserAuditDMLDatabase, this.chkUserAuditDML)
                            : null);
                    break;
                case DeselectControls.DbDatabaseSelect:
                    this.UpdateDependentCheckboxes(
                        currentControlChecked,
                        deselectOption,
                        deselectControl,
                        null,
                        currentControlChecked || (Globals.isServerNodeSelected ? !chkUserAuditSELECT.Checked : !_server.AuditUserSELECT)
                            ? this.GetConfigBasedCheckBox(this.chkUserAuditSELECTDatabase, this.chkUserAuditSELECT)
                            : null);
                    break;
                case DeselectControls.DbFilterEvents:
                    if (Globals.isServerNodeSelected ? chkUserAccessCheckFilter.Checked : _server.AuditUserAccessCheck != AccessCheckFilter.NoFilter)
                    {
                        break;
                    }
                    this.UpdateDependentCheckboxes(
                        currentControlChecked,
                        deselectOption,
                        deselectControl,
                        null,
                        this.GetConfigBasedCheckBox(this.chkUserAccessCheckFilterDatabase, this.chkUserAccessCheckFilter));
                    if (currentControlChecked)
                    {
                        // For Unchecked Values we don't have to set the radio buttons
                        this.UpdateDependentRadioButtons(
                            _rbDBAuditSuccessfulOnly.Checked,
                            deselectOption,
                            deselectControl,
                            null,
                            this.GetConfigBasedRadioButton(
                                this.rbUserAuditSuccessfulOnlyDatabase,
                                this.rbUserAuditSuccessfulOnly));
                        this.UpdateDependentRadioButtons(
                            _rbDBAuditFailedOnly.Checked,
                            deselectOption,
                            deselectControl,
                            null,
                            this.GetConfigBasedRadioButton(this.rbUserAuditFailedOnlyDatabase, this.rbUserAuditFailedOnly));
                    }
                    else
                    {
                        if (deselectOption == DeselectOptions.OtherLevels)
                        {
                            CheckCb(this.GetConfigBasedCheckBox(this.chkUserAccessCheckFilterDatabase, this.chkUserAccessCheckFilter), false);
                            EnableCb(this.GetConfigBasedCheckBox(this.chkUserAccessCheckFilterDatabase, this.chkUserAccessCheckFilter), true);
                            EnableRb(this.GetConfigBasedRadioButton(this.rbUserAuditFailedOnlyDatabase, this.rbUserAuditFailedOnly), false);
                            EnableRb(this.GetConfigBasedRadioButton(this.rbUserAuditSuccessfulOnlyDatabase, this.rbUserAuditSuccessfulOnly), false);
                        }
                        else
                        {
                            EnableCb(this.GetConfigBasedCheckBox(this.chkUserAccessCheckFilterDatabase, this.chkUserAccessCheckFilter), true);
                            if (this.GetConfigBasedCheckBox(this.chkUserAccessCheckFilterDatabase, this.chkUserAccessCheckFilter) != null && this.GetConfigBasedCheckBox(this.chkUserAccessCheckFilterDatabase, this.chkUserAccessCheckFilter).Checked)
                            {
                                EnableRb(this.GetConfigBasedRadioButton(this.rbUserAuditFailedOnlyDatabase, this.rbUserAuditFailedOnly), true);
                                EnableRb(this.GetConfigBasedRadioButton(this.rbUserAuditSuccessfulOnlyDatabase, this.rbUserAuditSuccessfulOnly), true);
                            }
                            else
                            {
                                EnableRb(this.GetConfigBasedRadioButton(this.rbUserAuditFailedOnlyDatabase, this.rbUserAuditFailedOnly), false);
                                EnableRb(this.GetConfigBasedRadioButton(this.rbUserAuditSuccessfulOnlyDatabase, this.rbUserAuditSuccessfulOnly), false);
                            }
                        }
                    }
                    break;
                case DeselectControls.DbFilterEventsPassOnly:
                    if (Globals.isServerNodeSelected ? chkUserAccessCheckFilter.Checked : _server.AuditUserAccessCheck != AccessCheckFilter.NoFilter)
                    {
                        break;
                    }
                    // For Unchecked Values we don't have to set the radio buttons
                    this.UpdateDependentRadioButtons(true, DeselectOptions.None, deselectControl, null, this.GetConfigBasedRadioButton(this.rbUserAuditSuccessfulOnlyDatabase, this.rbUserAuditSuccessfulOnly));
                    this.UpdateDependentRadioButtons(false, DeselectOptions.None, deselectControl, null, this.GetConfigBasedRadioButton(this.rbUserAuditFailedOnlyDatabase, this.rbUserAuditFailedOnly));
                    break;
                case DeselectControls.DbFilterEventsFailedOnly:
                    if (Globals.isServerNodeSelected ? chkUserAccessCheckFilter.Checked : _server.AuditUserAccessCheck != AccessCheckFilter.NoFilter)
                    {
                        break;
                    }
                    // For Unchecked Values we don't have to set the radio buttons
                    this.UpdateDependentRadioButtons(false, DeselectOptions.None, deselectControl, null, this.GetConfigBasedRadioButton(this.rbUserAuditSuccessfulOnlyDatabase, this.rbUserAuditSuccessfulOnly));
                    this.UpdateDependentRadioButtons(true, DeselectOptions.None, deselectControl, null, this.GetConfigBasedRadioButton(this.rbUserAuditFailedOnlyDatabase, this.rbUserAuditFailedOnly));
                    break;
                case DeselectControls.DbCaptureSqlDmlSelect:
                    EventHandler eventHandler = null;
                    if (Globals.isServerNodeSelected)
                    {
                        eventHandler = chkUserCaptureSQLDatabase_CheckedChanged;
                    }
                    else
                    {
                        eventHandler = chkUserCaptureSQL_CheckedChanged;
                    }
                    this.UpdateDependentCheckboxes(
                        currentControlChecked,
                        deselectOption,
                        deselectControl,
                        eventHandler,
                        currentControlChecked || (Globals.isServerNodeSelected ? !chkUserAuditCaptureSQL.Checked : !_server.AuditUserCaptureSQL)
                            ? this.GetConfigBasedCheckBox(this.chkUserAuditCaptureSQLDatabase, this.chkUserAuditCaptureSQL)
                            : null);
                    break;
                case DeselectControls.DbCaptureSqlTransactionStatus:
                    this.UpdateDependentCheckboxes(
                        currentControlChecked,
                        deselectOption,
                        deselectControl,
                        null,
                        currentControlChecked || (Globals.isServerNodeSelected ? !this.chkUserAuditCaptureTrans.Checked : !_server.AuditUserCaptureTrans)
                            ? this.GetConfigBasedCheckBox(this.chkUserAuditCaptureTransDatabase, this.chkUserAuditCaptureTrans)
                            : null);
                    break;
                case DeselectControls.DbCaptureSqlDdlSecurity:
                    this.UpdateDependentCheckboxes(
                        currentControlChecked,
                        deselectOption,
                        deselectControl,
                        null,
                        currentControlChecked || (Globals.isServerNodeSelected ? !chkUserCaptureDDL.Checked : !_server.AuditUserCaptureDDL)
                            ? this.GetConfigBasedCheckBox(this.chkUserCaptureDDLDatabase, this.chkUserCaptureDDL)
                            : null);
                    break;
                case DeselectControls.ServerUserLogins:
                    this.UpdateDependentCheckboxes(
                        currentControlChecked,
                        deselectOption,
                        deselectControl,
                        null,
                        this.GetConfigBasedCheckBox(this.chkUserAuditLoginsDatabase, null));
                    if (deselectOption == DeselectOptions.CurrentLevelOnly)
                    {
                        this.UpdateDependentCheckboxes(
                            currentControlChecked,
                            deselectOption,
                            deselectControl,
                            null,
                            this.GetConfigBasedCheckBox(this.chkUserAuditLogoutsDatabase, null));
                    }
                    break;
                case DeselectControls.ServerUserLogouts:
                    this.UpdateDependentCheckboxes(
                        currentControlChecked,
                        deselectOption,
                        deselectControl,
                        null,
                        this.GetConfigBasedCheckBox(this.chkUserAuditLogoutsDatabase, null));
                    break;
                case DeselectControls.ServerUserFailedLogins:
                    this.UpdateDependentCheckboxes(
                        currentControlChecked,
                        deselectOption,
                        deselectControl,
                        null,
                        this.GetConfigBasedCheckBox(this.chkUserAuditFailedLoginsDatabase, null));
                    break;
                case DeselectControls.ServerUserSecurityChanges:
                    this.UpdateDependentCheckboxes(
                        currentControlChecked,
                        deselectOption,
                        deselectControl,
                        null,
                        currentControlChecked || (Globals.isServerNodeSelected ? !_chkAuditSecurityChanges.Checked : (dbRecord == null || !dbRecord.AuditSecurity))
                            ? this.GetConfigBasedCheckBox(this.chkUserAuditSecurityDatabase, null)
                            : null);
                    break;
                case DeselectControls.ServerUserAdministrativeActivities:
                    this.UpdateDependentCheckboxes(
                        currentControlChecked,
                        deselectOption,
                        deselectControl,
                        null,
                        currentControlChecked || (Globals.isServerNodeSelected ? !_chkAuditAdminActivity.Checked : (dbRecord == null || !dbRecord.AuditAdmin))
                            ? this.GetConfigBasedCheckBox(this.chkUserAuditAdminDatabase, null)
                            : null);
                    break;
                case DeselectControls.ServerUserDatabaseDefinition:
                    this.UpdateDependentCheckboxes(
                        currentControlChecked,
                        deselectOption,
                        deselectControl,
                        null,
                        currentControlChecked || (Globals.isServerNodeSelected ? !_chkAuditDDL.Checked : (dbRecord == null || !dbRecord.AuditDDL))
                            ? this.GetConfigBasedCheckBox(this.chkUserAuditDDLDatabase, null) : null);
                    break;
                case DeselectControls.ServerUserDatabaseModifications:
                    this.UpdateDependentCheckboxes(
                        currentControlChecked,
                        deselectOption,
                        deselectControl,
                        null,
                        currentControlChecked || (Globals.isServerNodeSelected ? !_chkAuditDML.Checked : (dbRecord == null || !dbRecord.AuditDML))
                            ? this.GetConfigBasedCheckBox(this.chkUserAuditDMLDatabase, null)
                            : null);
                    break;
                case DeselectControls.ServerUserDatabaseSelect:
                    this.UpdateDependentCheckboxes(
                        currentControlChecked,
                        deselectOption,
                        deselectControl,
                        null,
                        currentControlChecked || (Globals.isServerNodeSelected ? !_chkAuditSelect.Checked : (dbRecord == null || !dbRecord.AuditSELECT))
                            ? this.GetConfigBasedCheckBox(this.chkUserAuditSELECTDatabase, null)
                            : null);
                    break;
                case DeselectControls.ServerUserUde:
                    this.UpdateDependentCheckboxes(
                        currentControlChecked,
                        deselectOption,
                        deselectControl,
                        null,
                        this.GetConfigBasedCheckBox(this.chkUserAuditUDEDatabase, null));
                    break;
                case DeselectControls.ServerUserCaptureSqlDmlSelect:
                    eventHandler = null;
                    if (Globals.isServerNodeSelected)
                    {
                        eventHandler = chkUserCaptureSQLDatabase_CheckedChanged;
                    }
                    else
                    {
                        eventHandler = chkUserCaptureSQL_CheckedChanged;
                    }
                    this.UpdateDependentCheckboxes(
                        currentControlChecked,
                        deselectOption,
                        deselectControl,
                        eventHandler,
                        currentControlChecked || (Globals.isServerNodeSelected ? !_chkCaptureSQL.Checked : (dbRecord == null || !dbRecord.AuditCaptureSQL))
                            ? this.GetConfigBasedCheckBox(this.chkUserAuditCaptureSQLDatabase, null)
                            : null);
                    break;
                case DeselectControls.ServerUserCaptureSqlTransactionStatus:
                    this.UpdateDependentCheckboxes(
                        currentControlChecked,
                        deselectOption,
                        deselectControl,
                        null,
                        currentControlChecked || (Globals.isServerNodeSelected ? !_chkCaptureTrans.Checked : (dbRecord == null || !dbRecord.AuditCaptureTrans))
                            ? this.GetConfigBasedCheckBox(this.chkUserAuditCaptureTransDatabase, null)
                            : null);
                    break;
                case DeselectControls.ServerUserCaptureSqlDdlSecurity:
                    this.UpdateDependentCheckboxes(
                        currentControlChecked,
                        deselectOption,
                        deselectControl,
                        null,
                        currentControlChecked || (Globals.isServerNodeSelected ? !_chkDBCaptureDDL.Checked : (dbRecord == null || !dbRecord.AuditCaptureDDL))
                            ? this.GetConfigBasedCheckBox(this.chkUserCaptureDDLDatabase, null)
                            : null);
                    break;
                case DeselectControls.ServerUserFilterEvents:
                    if (!currentControlChecked && (Globals.isServerNodeSelected ? _chkDBAccessCheckFilter.Checked : (dbRecord != null && dbRecord.AuditAccessCheck != AccessCheckFilter.NoFilter)))
                    {
                        // For Unchecked Values we don't have to set the radio buttons
                        this.UpdateDependentRadioButtons(Globals.isServerNodeSelected ? _rbDBAuditSuccessfulOnly.Checked : (dbRecord != null && dbRecord.AuditAccessCheck == AccessCheckFilter.SuccessOnly), DeselectOptions.None, deselectControl, null, this.GetConfigBasedRadioButton(this.rbUserAuditSuccessfulOnlyDatabase, this.rbUserAuditSuccessfulOnly));
                        this.UpdateDependentRadioButtons(Globals.isServerNodeSelected ? _rbDBAuditFailedOnly.Checked : (dbRecord != null && dbRecord.AuditAccessCheck == AccessCheckFilter.FailureOnly), DeselectOptions.None, deselectControl, null, this.GetConfigBasedRadioButton(this.rbUserAuditFailedOnlyDatabase, this.rbUserAuditFailedOnly));
                        break;
                    }
                    this.UpdateDependentCheckboxes(
                        currentControlChecked,
                        deselectOption,
                        deselectControl,
                        null,
                        this.GetConfigBasedCheckBox(this.chkUserAccessCheckFilterDatabase, null));
                    if (currentControlChecked)
                    {
                        // Ensuring that the Radio Buttons are enabled only if the parent checkboxes are enabled
                        if (chkUserAccessCheckFilter.Enabled)
                        {
                            // SQLCM-5661: Update the radio buttons passed and failed if the checkbox is mark checked on cancel
                            rbUserAuditSuccessfulOnly.Enabled = rbUserAuditFailedOnly.Enabled = true;
                        }
                        // For Unchecked Values we don't have to set the radio buttons
                        this.UpdateDependentRadioButtons(
                            rbUserAuditSuccessfulOnly.Checked,
                            deselectOption,
                            deselectControl,
                            null,
                            this.GetConfigBasedRadioButton(
                                this.rbUserAuditSuccessfulOnlyDatabase,
                                null));
                        this.UpdateDependentRadioButtons(
                            rbUserAuditFailedOnly.Checked,
                            deselectOption,
                            deselectControl,
                            null,
                            this.GetConfigBasedRadioButton(this.rbUserAuditFailedOnlyDatabase, null));
                    }
                    else
                    {
                        if (deselectOption == DeselectOptions.OtherLevels)
                        {
                            CheckCb(this.GetConfigBasedCheckBox(this.chkUserAccessCheckFilterDatabase, this.chkUserAccessCheckFilter), false);
                            EnableCb(this.GetConfigBasedCheckBox(this.chkUserAccessCheckFilterDatabase, this.chkUserAccessCheckFilter), true);
                            EnableRb(this.GetConfigBasedRadioButton(this.rbUserAuditFailedOnlyDatabase, this.rbUserAuditFailedOnly), false);
                            EnableRb(this.GetConfigBasedRadioButton(this.rbUserAuditSuccessfulOnlyDatabase, this.rbUserAuditSuccessfulOnly), false);
                        }
                        else
                        {
                            EnableCb(this.GetConfigBasedCheckBox(this.chkUserAccessCheckFilterDatabase, this.chkUserAccessCheckFilter), true);
                            if (this.GetConfigBasedCheckBox(this.chkUserAccessCheckFilterDatabase, this.chkUserAccessCheckFilter) != null && this.GetConfigBasedCheckBox(this.chkUserAccessCheckFilterDatabase, this.chkUserAccessCheckFilter).Checked)
                            {
                                EnableRb(this.GetConfigBasedRadioButton(this.rbUserAuditFailedOnlyDatabase, this.rbUserAuditFailedOnly), true);
                                EnableRb(this.GetConfigBasedRadioButton(this.rbUserAuditSuccessfulOnlyDatabase, this.rbUserAuditSuccessfulOnly), true);
                            }
                            else
                            {
                                EnableRb(this.GetConfigBasedRadioButton(this.rbUserAuditFailedOnlyDatabase, this.rbUserAuditFailedOnly), true);
                                EnableRb(this.GetConfigBasedRadioButton(this.rbUserAuditSuccessfulOnlyDatabase, this.rbUserAuditSuccessfulOnly), true);
                            }
                        }
                    }
                    break;
                case DeselectControls.ServerUserFilterEventsPassOnly:
                    // For Unchecked Values we don't have to set the radio buttons
                    this.UpdateDependentRadioButtons(true, DeselectOptions.None, deselectControl, null, this.GetConfigBasedRadioButton(this.rbUserAuditSuccessfulOnlyDatabase, this.rbUserAuditSuccessfulOnly));
                    this.UpdateDependentRadioButtons(false, DeselectOptions.None, deselectControl, null, this.GetConfigBasedRadioButton(this.rbUserAuditFailedOnlyDatabase, this.rbUserAuditFailedOnly));
                    break;
                case DeselectControls.ServerUserFilterEventsFailedOnly:
                    // For Unchecked Values we don't have to set the radio buttons
                    this.UpdateDependentRadioButtons(false, DeselectOptions.None, deselectControl, null, this.GetConfigBasedRadioButton(this.rbUserAuditSuccessfulOnlyDatabase, this.rbUserAuditSuccessfulOnly));
                    this.UpdateDependentRadioButtons(true, DeselectOptions.None, deselectControl, null, this.GetConfigBasedRadioButton(this.rbUserAuditFailedOnlyDatabase, this.rbUserAuditFailedOnly));
                    break;
            }
        }

        private CheckBox GetConfigBasedCheckBox(CheckBox serverConfiguration, CheckBox databaseConfiguration)
        {
            return Globals.isServerNodeSelected ? serverConfiguration : databaseConfiguration;
        }

        private RadioButton GetConfigBasedRadioButton(RadioButton serverConfiguration, RadioButton databaseConfiguration)
        {
            return Globals.isServerNodeSelected ? serverConfiguration : databaseConfiguration;
        }

        private void UpdateDependentRadioButtons(bool checkedValue, DeselectOptions deselectOptions, DeselectControls deselectControl, EventHandler checkedChangedHandler, params RadioButton[] dependentControls)
        {
            foreach (var dependentControl in dependentControls)
            {

                if (dependentControl == null)
                {
                    continue;
                }
                switch (deselectOptions)
                {
                    case DeselectOptions.CurrentLevelOnly:
                        dependentControl.Enabled = false;
                        break;
                    case DeselectOptions.None:
                    case DeselectOptions.OtherLevels:
                        dependentControl.Enabled = false;
                        var suppressCheckedChangedEventHandler =
                            this._deselectionManager.GetDeselectSuppressCheckBoxChanged(deselectControl);
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
        }

        private void UpdateDependentCheckboxes(bool checkedValue, DeselectOptions deselectOptions, DeselectControls deselectControl, EventHandler checkedChangedHandler, params CheckBox[] dependentControls)
        {
            foreach (var dependentControl in dependentControls)
            {
                if (dependentControl == null)
                {
                    continue;
                }
                switch (deselectOptions)
                {
                    case DeselectOptions.CurrentLevelOnly:
                        dependentControl.Enabled = !checkedValue;
                        break;
                    case DeselectOptions.None:
                    case DeselectOptions.OtherLevels:
                        dependentControl.Enabled = !checkedValue;
                        var suppressCheckedChangedEventHandler =
                            this._deselectionManager.GetDeselectSuppressCheckBoxChanged(deselectControl);
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
        }

        private void InitializeServerRegulationSettings(ServerRecord srv)
        {
            if (_templateSvrInitializationCompleted)
                return;
            ServerRecord tempServer = new ServerRecord();
            RegulationSettings settings;
            bool _privUserServerCheck = false;
            if (checkCustom.Checked)
            {
                if (_template.AuditTemplate.ServerLevelConfig != null)
                {
                    ServerAuditConfig _serverConfig = _template.AuditTemplate.ServerLevelConfig;
                    if (_serverConfig.Categories != null && _serverConfig.Categories.Length > 0)
                    {
                        foreach (AuditCategory cat in _serverConfig.Categories)
                            UpdateServerRecord(cat, tempServer);
                    }
                    tempServer.AuditAccessCheck = (AccessCheckFilter)_serverConfig.AccessCheckFilter;
                    if (_template.AuditTemplate.PrivUserConfig != null)
                    {
                        _privUserServerCheck = true;
                    }
                }
            }

            // apply PCI
            if (checkPCI.Checked)
            {
                if (_regulationSettings.TryGetValue((int)Regulation.RegulationType.PCI, out settings))
                {
                    tempServer.AuditLogins = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.Logins) == (int)RegulationSettings.RegulationServerCategory.Logins) || tempServer.AuditLogins;
                    // SQLCM-5375-6.1.4.3-Capture Logout Events at Server level - Updated the RegulationMap Data to include Logouts Events
                    tempServer.AuditLogouts = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.Logouts) == (int)RegulationSettings.RegulationServerCategory.Logouts) || tempServer.AuditLogouts;
                    tempServer.AuditFailedLogins = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.FailedLogins) == (int)RegulationSettings.RegulationServerCategory.FailedLogins) || tempServer.AuditFailedLogins;
                    tempServer.AuditDDL = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.DatabaseDefinition) == (int)RegulationSettings.RegulationServerCategory.DatabaseDefinition) || tempServer.AuditDDL;
                    tempServer.AuditAdmin = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.AdminActivity) == (int)RegulationSettings.RegulationServerCategory.AdminActivity) || tempServer.AuditAdmin;
                    tempServer.AuditSecurity = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.SecurityChanges) == (int)RegulationSettings.RegulationServerCategory.SecurityChanges) || tempServer.AuditSecurity;
                    tempServer.AuditUDE = false || tempServer.AuditSecurity;

                    if ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.FilterPassedAccess) == (int)RegulationSettings.RegulationServerCategory.FilterPassedAccess || (tempServer.AuditAccessCheck == AccessCheckFilter.SuccessOnly))
                        tempServer.AuditAccessCheck = AccessCheckFilter.SuccessOnly;
                    else if ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.FilterFailedAccess) == (int)RegulationSettings.RegulationServerCategory.FilterFailedAccess || (tempServer.AuditAccessCheck == AccessCheckFilter.FailureOnly))
                        tempServer.AuditAccessCheck = AccessCheckFilter.FailureOnly;
                    else
                        tempServer.AuditAccessCheck = AccessCheckFilter.NoFilter;

                    _privUserServerCheck = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.PrivelegedUsers) == (int)RegulationSettings.RegulationServerCategory.PrivelegedUsers) || _privUserServerCheck;
                }
            }

            // apply HIPAA
            // the OR against the server settings is done because the user can select more than one template.  When more than one template is 
            // selected, the options are combined together
            if (checkHIPAA.Checked)
            {
                if (_regulationSettings.TryGetValue((int)Regulation.RegulationType.HIPAA, out settings))
                {
                    tempServer.AuditLogins = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.Logins) == (int)RegulationSettings.RegulationServerCategory.Logins) || tempServer.AuditLogins;
                    // SQLCM-5375-6.1.4.3-Capture Logout Events at Server level - Updated the RegulationMap Data to include Logouts Events
                    tempServer.AuditLogouts = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.Logouts) == (int)RegulationSettings.RegulationServerCategory.Logouts) || tempServer.AuditLogouts;
                    tempServer.AuditFailedLogins = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.FailedLogins) == (int)RegulationSettings.RegulationServerCategory.FailedLogins) || tempServer.AuditFailedLogins;
                    tempServer.AuditDDL = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.DatabaseDefinition) == (int)RegulationSettings.RegulationServerCategory.DatabaseDefinition) || tempServer.AuditDDL;
                    tempServer.AuditAdmin = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.AdminActivity) == (int)RegulationSettings.RegulationServerCategory.AdminActivity) || tempServer.AuditAdmin;
                    tempServer.AuditSecurity = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.SecurityChanges) == (int)RegulationSettings.RegulationServerCategory.SecurityChanges) || tempServer.AuditSecurity;
                    tempServer.AuditUDE = false;


                    if (((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.FilterPassedAccess) == (int)RegulationSettings.RegulationServerCategory.FilterPassedAccess) || (tempServer.AuditAccessCheck == AccessCheckFilter.SuccessOnly))
                        tempServer.AuditAccessCheck = AccessCheckFilter.SuccessOnly;
                    else if (((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.FilterFailedAccess) == (int)RegulationSettings.RegulationServerCategory.FilterFailedAccess) || (tempServer.AuditAccessCheck == AccessCheckFilter.FailureOnly))
                        tempServer.AuditAccessCheck = AccessCheckFilter.FailureOnly;
                    else
                        tempServer.AuditAccessCheck = AccessCheckFilter.NoFilter;

                    _privUserServerCheck = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.PrivelegedUsers) == (int)RegulationSettings.RegulationServerCategory.PrivelegedUsers) || _privUserServerCheck;
                }
            }

            if (checkDISA.Checked)
            {
                if (_regulationSettings.TryGetValue((int)Regulation.RegulationType.DISA, out settings))
                {
                    tempServer.AuditLogins = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.Logins) == (int)RegulationSettings.RegulationServerCategory.Logins) || tempServer.AuditLogins;
                    // SQLCM-5375-6.1.4.3-Capture Logout Events at Server level - Updated the RegulationMap Data to include Logouts Events
                    tempServer.AuditLogouts = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.Logouts) == (int)RegulationSettings.RegulationServerCategory.Logouts) || tempServer.AuditLogouts;
                    tempServer.AuditFailedLogins = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.FailedLogins) == (int)RegulationSettings.RegulationServerCategory.FailedLogins) || tempServer.AuditFailedLogins;
                    tempServer.AuditDDL = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.DatabaseDefinition) == (int)RegulationSettings.RegulationServerCategory.DatabaseDefinition) || tempServer.AuditDDL;
                    tempServer.AuditAdmin = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.AdminActivity) == (int)RegulationSettings.RegulationServerCategory.AdminActivity) || tempServer.AuditAdmin;
                    tempServer.AuditSecurity = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.SecurityChanges) == (int)RegulationSettings.RegulationServerCategory.SecurityChanges) || tempServer.AuditSecurity;
                    tempServer.AuditUDE = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.UserDefined) == (int)RegulationSettings.RegulationServerCategory.UserDefined) || tempServer.AuditUDE;

                    if (((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.FilterPassedAccess) == (int)RegulationSettings.RegulationServerCategory.FilterPassedAccess) || (tempServer.AuditAccessCheck == AccessCheckFilter.SuccessOnly))
                        tempServer.AuditAccessCheck = AccessCheckFilter.SuccessOnly;
                    else if (((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.FilterFailedAccess) == (int)RegulationSettings.RegulationServerCategory.FilterFailedAccess) || (tempServer.AuditAccessCheck == AccessCheckFilter.FailureOnly))
                        tempServer.AuditAccessCheck = AccessCheckFilter.FailureOnly;
                    else
                        tempServer.AuditAccessCheck = AccessCheckFilter.NoFilter;

                    _privUserServerCheck = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.PrivelegedUsers) == (int)RegulationSettings.RegulationServerCategory.PrivelegedUsers) || _privUserServerCheck;
                }
            }

            if (checkNERC.Checked)
            {
                if (_regulationSettings.TryGetValue((int)Regulation.RegulationType.NERC, out settings))
                {
                    tempServer.AuditLogins = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.Logins) == (int)RegulationSettings.RegulationServerCategory.Logins) || tempServer.AuditLogins;
                    // SQLCM-5375-6.1.4.3-Capture Logout Events at Server level - Updated the RegulationMap Data to include Logouts Events
                    tempServer.AuditLogouts = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.Logouts) == (int)RegulationSettings.RegulationServerCategory.Logouts) || tempServer.AuditLogouts;
                    tempServer.AuditFailedLogins = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.FailedLogins) == (int)RegulationSettings.RegulationServerCategory.FailedLogins) || tempServer.AuditFailedLogins;
                    tempServer.AuditDDL = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.DatabaseDefinition) == (int)RegulationSettings.RegulationServerCategory.DatabaseDefinition) || tempServer.AuditDDL;
                    tempServer.AuditAdmin = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.AdminActivity) == (int)RegulationSettings.RegulationServerCategory.AdminActivity) || tempServer.AuditAdmin;
                    tempServer.AuditSecurity = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.SecurityChanges) == (int)RegulationSettings.RegulationServerCategory.SecurityChanges) || tempServer.AuditSecurity;
                    tempServer.AuditUDE = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.UserDefined) == (int)RegulationSettings.RegulationServerCategory.UserDefined) || tempServer.AuditUDE;

                    if (((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.FilterPassedAccess) == (int)RegulationSettings.RegulationServerCategory.FilterPassedAccess) || (tempServer.AuditAccessCheck == AccessCheckFilter.SuccessOnly))
                        tempServer.AuditAccessCheck = AccessCheckFilter.SuccessOnly;
                    else if (((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.FilterFailedAccess) == (int)RegulationSettings.RegulationServerCategory.FilterFailedAccess) || (tempServer.AuditAccessCheck == AccessCheckFilter.FailureOnly))
                        tempServer.AuditAccessCheck = AccessCheckFilter.FailureOnly;
                    else
                        tempServer.AuditAccessCheck = AccessCheckFilter.NoFilter;

                    _privUserServerCheck = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.PrivelegedUsers) == (int)RegulationSettings.RegulationServerCategory.PrivelegedUsers) || _privUserServerCheck;
                }
            }

            if (checkCIS.Checked)
            {
                if (_regulationSettings.TryGetValue((int)Regulation.RegulationType.CIS, out settings))
                {
                    tempServer.AuditLogins = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.Logins) == (int)RegulationSettings.RegulationServerCategory.Logins) || tempServer.AuditLogins;
                    // SQLCM-5375-6.1.4.3-Capture Logout Events at Server level - Updated the RegulationMap Data to include Logouts Events
                    tempServer.AuditLogouts = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.Logouts) == (int)RegulationSettings.RegulationServerCategory.Logouts) || tempServer.AuditLogouts;
                    tempServer.AuditFailedLogins = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.FailedLogins) == (int)RegulationSettings.RegulationServerCategory.FailedLogins) || tempServer.AuditFailedLogins;
                    tempServer.AuditDDL = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.DatabaseDefinition) == (int)RegulationSettings.RegulationServerCategory.DatabaseDefinition) || tempServer.AuditDDL;
                    tempServer.AuditAdmin = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.AdminActivity) == (int)RegulationSettings.RegulationServerCategory.AdminActivity) || tempServer.AuditAdmin;
                    tempServer.AuditSecurity = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.SecurityChanges) == (int)RegulationSettings.RegulationServerCategory.SecurityChanges) || tempServer.AuditSecurity;
                    tempServer.AuditUDE = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.UserDefined) == (int)RegulationSettings.RegulationServerCategory.UserDefined) || tempServer.AuditUDE;

                    if (((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.FilterPassedAccess) == (int)RegulationSettings.RegulationServerCategory.FilterPassedAccess) || (tempServer.AuditAccessCheck == AccessCheckFilter.SuccessOnly))
                        tempServer.AuditAccessCheck = AccessCheckFilter.SuccessOnly;
                    else if (((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.FilterFailedAccess) == (int)RegulationSettings.RegulationServerCategory.FilterFailedAccess) || (tempServer.AuditAccessCheck == AccessCheckFilter.FailureOnly))
                        tempServer.AuditAccessCheck = AccessCheckFilter.FailureOnly;
                    else
                        tempServer.AuditAccessCheck = AccessCheckFilter.NoFilter;

                    _privUserServerCheck = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.PrivelegedUsers) == (int)RegulationSettings.RegulationServerCategory.PrivelegedUsers) || _privUserServerCheck;
                }
            }

            if (checkSOX.Checked)
            {
                if (_regulationSettings.TryGetValue((int)Regulation.RegulationType.SOX, out settings))
                {
                    tempServer.AuditLogins = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.Logins) == (int)RegulationSettings.RegulationServerCategory.Logins) || tempServer.AuditLogins;
                    // SQLCM-5375-6.1.4.3-Capture Logout Events at Server level - Updated the RegulationMap Data to include Logouts Events
                    tempServer.AuditLogouts = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.Logouts) == (int)RegulationSettings.RegulationServerCategory.Logouts) || tempServer.AuditLogouts;
                    tempServer.AuditFailedLogins = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.FailedLogins) == (int)RegulationSettings.RegulationServerCategory.FailedLogins) || tempServer.AuditFailedLogins;
                    tempServer.AuditDDL = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.DatabaseDefinition) == (int)RegulationSettings.RegulationServerCategory.DatabaseDefinition) || tempServer.AuditDDL;
                    tempServer.AuditAdmin = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.AdminActivity) == (int)RegulationSettings.RegulationServerCategory.AdminActivity) || tempServer.AuditAdmin;
                    tempServer.AuditSecurity = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.SecurityChanges) == (int)RegulationSettings.RegulationServerCategory.SecurityChanges) || tempServer.AuditSecurity;
                    tempServer.AuditUDE = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.UserDefined) == (int)RegulationSettings.RegulationServerCategory.UserDefined) || tempServer.AuditUDE;

                    if (((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.FilterPassedAccess) == (int)RegulationSettings.RegulationServerCategory.FilterPassedAccess) || (tempServer.AuditAccessCheck == AccessCheckFilter.SuccessOnly))
                        tempServer.AuditAccessCheck = AccessCheckFilter.SuccessOnly;
                    else if (((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.FilterFailedAccess) == (int)RegulationSettings.RegulationServerCategory.FilterFailedAccess) || (tempServer.AuditAccessCheck == AccessCheckFilter.FailureOnly))
                        tempServer.AuditAccessCheck = AccessCheckFilter.FailureOnly;
                    else
                        tempServer.AuditAccessCheck = AccessCheckFilter.NoFilter;

                    _privUserServerCheck = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.PrivelegedUsers) == (int)RegulationSettings.RegulationServerCategory.PrivelegedUsers) || _privUserServerCheck;
                }
            }
            if (checkFERPA.Checked)
            {
                if (_regulationSettings.TryGetValue((int)Regulation.RegulationType.FERPA, out settings))
                {
                    tempServer.AuditLogins = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.Logins) == (int)RegulationSettings.RegulationServerCategory.Logins) || tempServer.AuditLogins;
                    tempServer.AuditLogouts = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.Logouts) == (int)RegulationSettings.RegulationServerCategory.Logouts) || tempServer.AuditLogouts;
                    tempServer.AuditFailedLogins = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.FailedLogins) == (int)RegulationSettings.RegulationServerCategory.FailedLogins) || tempServer.AuditFailedLogins;
                    tempServer.AuditDDL = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.DatabaseDefinition) == (int)RegulationSettings.RegulationServerCategory.DatabaseDefinition) || tempServer.AuditDDL;
                    tempServer.AuditAdmin = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.AdminActivity) == (int)RegulationSettings.RegulationServerCategory.AdminActivity) || tempServer.AuditAdmin;
                    tempServer.AuditSecurity = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.SecurityChanges) == (int)RegulationSettings.RegulationServerCategory.SecurityChanges) || tempServer.AuditSecurity;
                    tempServer.AuditUDE = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.UserDefined) == (int)RegulationSettings.RegulationServerCategory.UserDefined) || tempServer.AuditUDE;

                    if (((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.FilterPassedAccess) == (int)RegulationSettings.RegulationServerCategory.FilterPassedAccess) || (tempServer.AuditAccessCheck == AccessCheckFilter.SuccessOnly))
                        tempServer.AuditAccessCheck = AccessCheckFilter.SuccessOnly;
                    else if (((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.FilterFailedAccess) == (int)RegulationSettings.RegulationServerCategory.FilterFailedAccess) || (tempServer.AuditAccessCheck == AccessCheckFilter.FailureOnly))
                        tempServer.AuditAccessCheck = AccessCheckFilter.FailureOnly;
                    else
                        tempServer.AuditAccessCheck = AccessCheckFilter.NoFilter;

                    _privUserServerCheck = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.PrivelegedUsers) == (int)RegulationSettings.RegulationServerCategory.PrivelegedUsers) || _privUserServerCheck;
                }
            }
            if (checkGDPR.Checked)
            {
                if (_regulationSettings.TryGetValue((int)Regulation.RegulationType.GDPR, out settings))
                {
                    tempServer.AuditLogins = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.Logins) == (int)RegulationSettings.RegulationServerCategory.Logins) || tempServer.AuditLogins;
                    tempServer.AuditLogouts = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.Logouts) == (int)RegulationSettings.RegulationServerCategory.Logouts) || tempServer.AuditLogouts;
                    tempServer.AuditFailedLogins = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.FailedLogins) == (int)RegulationSettings.RegulationServerCategory.FailedLogins) || tempServer.AuditFailedLogins;
                    tempServer.AuditDDL = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.DatabaseDefinition) == (int)RegulationSettings.RegulationServerCategory.DatabaseDefinition) || tempServer.AuditDDL;
                    tempServer.AuditAdmin = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.AdminActivity) == (int)RegulationSettings.RegulationServerCategory.AdminActivity) || tempServer.AuditAdmin;
                    tempServer.AuditSecurity = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.SecurityChanges) == (int)RegulationSettings.RegulationServerCategory.SecurityChanges) || tempServer.AuditSecurity;
                    tempServer.AuditUDE = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.UserDefined) == (int)RegulationSettings.RegulationServerCategory.UserDefined) || tempServer.AuditUDE;

                    if (((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.FilterPassedAccess) == (int)RegulationSettings.RegulationServerCategory.FilterPassedAccess) || (tempServer.AuditAccessCheck == AccessCheckFilter.SuccessOnly))
                        tempServer.AuditAccessCheck = AccessCheckFilter.SuccessOnly;
                    else if (((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.FilterFailedAccess) == (int)RegulationSettings.RegulationServerCategory.FilterFailedAccess) || (tempServer.AuditAccessCheck == AccessCheckFilter.FailureOnly))
                        tempServer.AuditAccessCheck = AccessCheckFilter.FailureOnly;
                    else
                        tempServer.AuditAccessCheck = AccessCheckFilter.NoFilter;

                    _privUserServerCheck = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.PrivelegedUsers) == (int)RegulationSettings.RegulationServerCategory.PrivelegedUsers) || _privUserServerCheck;
                    // SQlCM 5372 - GDPR 
                    if (_privUserServerCheck)
                    {
                        tempServer.AuditUserAccessCheck = AccessCheckFilter.SuccessOnly;
                    }
                }
            }

            DeselectionManager.SetAdditiveCheckbox(tempServer.AuditLogins, _checkLogins, this.chkUserAuditLogins, this.GetConfigBasedCheckBox(this.chkUserAuditLoginsDatabase, this.chkUserAuditLogins));
            DeselectionManager.SetAndGreyOutAdditiveCheckbox(tempServer.AuditLogins, this.chkUserAuditLogins, this.GetConfigBasedCheckBox(this.chkUserAuditLoginsDatabase, this.chkUserAuditLogins));

            DeselectionManager.SetAdditiveCheckbox(tempServer.AuditLogouts, _checkLogouts, this.chkUserAuditLogouts, this.GetConfigBasedCheckBox(this.chkUserAuditLogoutsDatabase, this.chkUserAuditLogouts));
            DeselectionManager.SetAndGreyOutAdditiveCheckbox(tempServer.AuditLogouts, this.chkUserAuditLogouts, this.GetConfigBasedCheckBox(this.chkUserAuditLogoutsDatabase, this.chkUserAuditLogouts));

            DeselectionManager.SetAdditiveCheckbox(tempServer.AuditFailedLogins, _checkFailedLogins, this.chkUserAuditFailedLogins, this.GetConfigBasedCheckBox(this.chkUserAuditFailedLoginsDatabase, this.chkUserAuditFailedLogins));
            DeselectionManager.SetAndGreyOutAdditiveCheckbox(tempServer.AuditFailedLogins, this.chkUserAuditFailedLogins, this.GetConfigBasedCheckBox(this.chkUserAuditFailedLoginsDatabase, this.chkUserAuditFailedLogins));

            DeselectionManager.SetAdditiveCheckbox(tempServer.AuditDDL, _checkDDL, _chkAuditDDL, this.chkUserAuditDDL, this.GetConfigBasedCheckBox(this.chkUserAuditDDLDatabase, this.chkUserAuditDDL));
            DeselectionManager.SetAndGreyOutAdditiveCheckbox(tempServer.AuditDDL, _chkAuditDDL, this.chkUserAuditDDL, this.GetConfigBasedCheckBox(this.chkUserAuditDDLDatabase, this.chkUserAuditDDL));

            DeselectionManager.SetAdditiveCheckbox(tempServer.AuditAdmin, _checkAdministrativeActivities, this.chkUserAuditAdmin, _chkAuditAdminActivity, this.GetConfigBasedCheckBox(this.chkUserAuditAdminDatabase, this.chkUserAuditAdmin));
            DeselectionManager.SetAndGreyOutAdditiveCheckbox(tempServer.AuditAdmin, this.chkUserAuditAdmin, _chkAuditAdminActivity, this.GetConfigBasedCheckBox(this.chkUserAuditAdminDatabase, this.chkUserAuditAdmin));

            DeselectionManager.SetAdditiveCheckbox(tempServer.AuditSecurity, _checkSecurityChanges, this.chkUserAuditSecurity, this._chkAuditSecurityChanges, this.GetConfigBasedCheckBox(this.chkUserAuditSecurityDatabase, this.chkUserAuditSecurity));
            DeselectionManager.SetAndGreyOutAdditiveCheckbox(tempServer.AuditSecurity, this.chkUserAuditSecurity, this._chkAuditSecurityChanges, this.GetConfigBasedCheckBox(this.chkUserAuditSecurityDatabase, this.chkUserAuditSecurity));

            DeselectionManager.SetAdditiveCheckbox(tempServer.AuditUDE, _checkUserDefinedEvents, this.chkUserAuditUDE, this.GetConfigBasedCheckBox(this.chkUserAuditUDEDatabase, this.chkUserAuditUDE));
            DeselectionManager.SetAndGreyOutAdditiveCheckbox(tempServer.AuditUDE, this.chkUserAuditUDE, this.GetConfigBasedCheckBox(this.chkUserAuditUDEDatabase, this.chkUserAuditUDE));

            checkPriviligedUser.Checked = _privUserServerCheck;  // To check priv User at server level is enable by regulation guideline.
            this._chkPriviligedUser.Checked = _privUserServerCheck;

            if (_server.AuditAccessCheck == AccessCheckFilter.NoFilter)
            {
                if (tempServer.AuditAccessCheck == AccessCheckFilter.FailureOnly)
                {
                    CheckRb(this.GetConfigBasedRadioButton(this.rbUserAuditFailedOnlyDatabase, this.rbUserAuditFailedOnly), true);
                    _rbDBAuditFailedOnly.Checked = rbUserAuditFailedOnly.Checked = _rbAuditFailedOnly.Checked = true;

                    CheckCb(this.GetConfigBasedCheckBox(this.chkUserAccessCheckFilterDatabase, this.chkUserAccessCheckFilter), true);
                    _chkDBAccessCheckFilter.Checked = _chkServerAccessCheckFilter.Checked = chkUserAccessCheckFilter.Checked = true;

                    EnableCb(this.GetConfigBasedCheckBox(this.chkUserAccessCheckFilterDatabase, this.chkUserAccessCheckFilter), false);
                    _chkDBAccessCheckFilter.Enabled = chkUserAccessCheckFilter.Enabled = false;

                    EnableRb(this.GetConfigBasedRadioButton(this.rbUserAuditFailedOnlyDatabase, this.rbUserAuditFailedOnly), false);
                    _rbDBAuditFailedOnly.Enabled = rbUserAuditFailedOnly.Enabled = false;
                    _rbAuditFailedOnly.Enabled = true;

                    EnableRb(this.GetConfigBasedRadioButton(this.rbUserAuditSuccessfulOnlyDatabase, this.rbUserAuditSuccessfulOnly), false);
                    _rbDBAuditSuccessfulOnly.Enabled = rbUserAuditSuccessfulOnly.Enabled = false;
                    _rbServerAuditSuccessfulOnly.Enabled = true;
                }
                else if (tempServer.AuditAccessCheck == AccessCheckFilter.SuccessOnly)
                {
                    CheckRb(this.GetConfigBasedRadioButton(this.rbUserAuditSuccessfulOnlyDatabase, this.rbUserAuditSuccessfulOnly), true);
                    _rbDBAuditSuccessfulOnly.Checked = rbUserAuditSuccessfulOnly.Checked = _rbServerAuditSuccessfulOnly.Checked = true;

                    CheckCb(this.GetConfigBasedCheckBox(this.chkUserAccessCheckFilterDatabase, this.chkUserAccessCheckFilter), true);
                    _chkDBAccessCheckFilter.Checked = chkUserAccessCheckFilter.Checked = _chkServerAccessCheckFilter.Checked = true;

                    EnableCb(this.GetConfigBasedCheckBox(this.chkUserAccessCheckFilterDatabase, this.chkUserAccessCheckFilter), false);
                    _chkDBAccessCheckFilter.Enabled = chkUserAccessCheckFilter.Enabled = false;

                    EnableRb(this.GetConfigBasedRadioButton(this.rbUserAuditFailedOnlyDatabase, this.rbUserAuditFailedOnly), false);
                    _rbDBAuditFailedOnly.Enabled = rbUserAuditFailedOnly.Enabled = false;
                    _rbAuditFailedOnly.Enabled = true;

                    EnableRb(this.GetConfigBasedRadioButton(this.rbUserAuditSuccessfulOnlyDatabase, this.rbUserAuditSuccessfulOnly), false);
                    _rbDBAuditSuccessfulOnly.Enabled = rbUserAuditSuccessfulOnly.Enabled = false;
                    _rbServerAuditSuccessfulOnly.Enabled = true;
                }
            }

        }

        private void InitializeDBRegulationSettings()
        {
            if (_templateDBInitializationCompleted)
                return;

            RegulationSettings settings;
            DatabaseRecord tempDb = new DatabaseRecord();
            bool _privUserDbCheck = false;
            if (checkCustom.Checked)
            {
                if (_template.AuditTemplate.DbLevelConfigs != null)
                {
                    DBAuditConfig[] config = _template.AuditTemplate.DbLevelConfigs;
                    if (config != null && config.Length > 0)
                    {
                        UpdateDBRecord(tempDb, config[0]);
                    }

                    foreach (DBAuditConfig item in config)
                    {
                        if (item.PrivUserConfig != null)
                        {
                            _privUserDbCheck = true;
                        }
                        if (item.SensitiveColumnTables != null && item.SensitiveColumnTables.Length > 0)
                        {
                            tempDb.AuditSensitiveColumns = true;
                        }
                        if (item.DataChangeTables != null && item.DataChangeTables.Length > 0)
                        {
                            tempDb.AuditDataChanges = true;
                        }
                    }
                }
            }
            // apply PCI
            if (checkPCI.Checked)
            {
                if (_regulationSettings.TryGetValue((int)Regulation.RegulationType.PCI, out settings))
                {
                    tempDb.AuditDDL = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.DatabaseDefinition) == (int)RegulationSettings.RegulationDatabaseCategory.DatabaseDefinition) || tempDb.AuditDDL;
                    tempDb.AuditSecurity = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.SecurityChanges) == (int)RegulationSettings.RegulationDatabaseCategory.SecurityChanges) || tempDb.AuditSecurity;
                    tempDb.AuditAdmin = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.AdminActivity) == (int)RegulationSettings.RegulationDatabaseCategory.AdminActivity) || tempDb.AuditAdmin;
                    tempDb.AuditDML = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.DatabaseModification) == (int)RegulationSettings.RegulationDatabaseCategory.DatabaseModification) || tempDb.AuditDML;
                    tempDb.AuditSELECT = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.Select) == (int)RegulationSettings.RegulationDatabaseCategory.Select) || tempDb.AuditSELECT;

                    if (((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.FilterPassedAccess) == (int)RegulationSettings.RegulationDatabaseCategory.FilterPassedAccess) || (tempDb.AuditAccessCheck == AccessCheckFilter.SuccessOnly))
                        tempDb.AuditAccessCheck = AccessCheckFilter.SuccessOnly;
                    else if (((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.FilterFailedAccess) == (int)RegulationSettings.RegulationDatabaseCategory.FilterFailedAccess) || (tempDb.AuditAccessCheck == AccessCheckFilter.FailureOnly))
                        tempDb.AuditAccessCheck = AccessCheckFilter.FailureOnly;
                    else
                        tempDb.AuditAccessCheck = AccessCheckFilter.NoFilter;

                    tempDb.AuditCaptureSQL = CoreConstants.AllowCaptureSql && ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.SQLText) == (int)RegulationSettings.RegulationDatabaseCategory.SQLText) || tempDb.AuditCaptureSQL;
                    tempDb.AuditCaptureTrans = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.Transactions) == (int)RegulationSettings.RegulationDatabaseCategory.Transactions) || tempDb.AuditCaptureTrans;
                    tempDb.AuditSensitiveColumns = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.SensitiveColumns) == (int)RegulationSettings.RegulationDatabaseCategory.SensitiveColumns) || tempDb.AuditSensitiveColumns;
                    tempDb.AuditDataChanges = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.BeforeAfterDataChange) == (int)RegulationSettings.RegulationDatabaseCategory.BeforeAfterDataChange) || tempDb.AuditDataChanges;

                    _privUserDbCheck = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.PrivelegedUsers) == (int)RegulationSettings.RegulationDatabaseCategory.PrivelegedUsers) || _privUserDbCheck;
                }
            }
            // apply HIPAA
            // the OR against the server settings is done because the user can select more than one template.  When more than one template is 
            // selected, the options are combined togeher
            if (checkHIPAA.Checked)
            {
                if (_regulationSettings.TryGetValue((int)Regulation.RegulationType.HIPAA, out settings))
                {
                    tempDb.AuditDDL = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.DatabaseDefinition) == (int)RegulationSettings.RegulationDatabaseCategory.DatabaseDefinition) || tempDb.AuditDDL;
                    tempDb.AuditSecurity = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.SecurityChanges) == (int)RegulationSettings.RegulationDatabaseCategory.SecurityChanges) || tempDb.AuditSecurity;
                    tempDb.AuditAdmin = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.AdminActivity) == (int)RegulationSettings.RegulationDatabaseCategory.AdminActivity) || tempDb.AuditAdmin;
                    tempDb.AuditDML = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.DatabaseModification) == (int)RegulationSettings.RegulationDatabaseCategory.DatabaseModification) || tempDb.AuditDML;
                    tempDb.AuditSELECT = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.Select) == (int)RegulationSettings.RegulationDatabaseCategory.Select) || tempDb.AuditSELECT;

                    if (((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.FilterPassedAccess) == (int)RegulationSettings.RegulationDatabaseCategory.FilterPassedAccess) || (tempDb.AuditAccessCheck == AccessCheckFilter.SuccessOnly))
                        tempDb.AuditAccessCheck = AccessCheckFilter.SuccessOnly;
                    else if (((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.FilterFailedAccess) == (int)RegulationSettings.RegulationDatabaseCategory.FilterFailedAccess) || (tempDb.AuditAccessCheck == AccessCheckFilter.FailureOnly))
                        tempDb.AuditAccessCheck = AccessCheckFilter.FailureOnly;
                    else
                        tempDb.AuditAccessCheck = AccessCheckFilter.NoFilter;

                    tempDb.AuditCaptureSQL = (CoreConstants.AllowCaptureSql && ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.SQLText) == (int)RegulationSettings.RegulationDatabaseCategory.SQLText)) || tempDb.AuditCaptureSQL;
                    tempDb.AuditCaptureTrans = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.Transactions) == (int)RegulationSettings.RegulationDatabaseCategory.Transactions) || tempDb.AuditCaptureTrans;
                    tempDb.AuditSensitiveColumns = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.SensitiveColumns) == (int)RegulationSettings.RegulationDatabaseCategory.SensitiveColumns) || tempDb.AuditSensitiveColumns;
                    tempDb.AuditDataChanges = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.BeforeAfterDataChange) == (int)RegulationSettings.RegulationDatabaseCategory.BeforeAfterDataChange) || tempDb.AuditDataChanges;

                    _privUserDbCheck = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.PrivelegedUsers) == (int)RegulationSettings.RegulationDatabaseCategory.PrivelegedUsers) || _privUserDbCheck;

                }
            }

            if (checkDISA.Checked)
            {

                if (_regulationSettings.TryGetValue((int)Regulation.RegulationType.DISA, out settings))
                {
                    tempDb.AuditDDL = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.DatabaseDefinition) == (int)RegulationSettings.RegulationDatabaseCategory.DatabaseDefinition) || tempDb.AuditDDL;
                    tempDb.AuditSecurity = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.SecurityChanges) == (int)RegulationSettings.RegulationDatabaseCategory.SecurityChanges) || tempDb.AuditSecurity;
                    tempDb.AuditAdmin = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.AdminActivity) == (int)RegulationSettings.RegulationDatabaseCategory.AdminActivity) || tempDb.AuditAdmin;
                    tempDb.AuditDML = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.DatabaseModification) == (int)RegulationSettings.RegulationDatabaseCategory.DatabaseModification) || tempDb.AuditDML;
                    tempDb.AuditSELECT = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.Select) == (int)RegulationSettings.RegulationDatabaseCategory.Select) || tempDb.AuditSELECT;

                    if (((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.FilterPassedAccess) == (int)RegulationSettings.RegulationDatabaseCategory.FilterPassedAccess) || (tempDb.AuditAccessCheck == AccessCheckFilter.SuccessOnly))
                        tempDb.AuditAccessCheck = AccessCheckFilter.SuccessOnly;
                    else if (((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.FilterFailedAccess) == (int)RegulationSettings.RegulationDatabaseCategory.FilterFailedAccess) || (tempDb.AuditAccessCheck == AccessCheckFilter.FailureOnly))
                        tempDb.AuditAccessCheck = AccessCheckFilter.FailureOnly;
                    else
                        tempDb.AuditAccessCheck = AccessCheckFilter.NoFilter;

                    tempDb.AuditCaptureSQL = (CoreConstants.AllowCaptureSql && ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.SQLText) == (int)RegulationSettings.RegulationDatabaseCategory.SQLText)) || tempDb.AuditCaptureSQL;
                    tempDb.AuditCaptureTrans = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.Transactions) == (int)RegulationSettings.RegulationDatabaseCategory.Transactions) || tempDb.AuditCaptureTrans;
                    tempDb.AuditSensitiveColumns = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.SensitiveColumns) == (int)RegulationSettings.RegulationDatabaseCategory.SensitiveColumns) || tempDb.AuditSensitiveColumns;
                    tempDb.AuditDataChanges = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.BeforeAfterDataChange) == (int)RegulationSettings.RegulationDatabaseCategory.BeforeAfterDataChange) || tempDb.AuditDataChanges;

                    _privUserDbCheck = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.PrivelegedUsers) == (int)RegulationSettings.RegulationDatabaseCategory.PrivelegedUsers) || _privUserDbCheck;

                }
            }

            if (checkNERC.Checked)
            {
                if (_regulationSettings.TryGetValue((int)Regulation.RegulationType.NERC, out settings))
                {
                    tempDb.AuditDDL = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.DatabaseDefinition) == (int)RegulationSettings.RegulationDatabaseCategory.DatabaseDefinition) || tempDb.AuditDDL;
                    tempDb.AuditSecurity = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.SecurityChanges) == (int)RegulationSettings.RegulationDatabaseCategory.SecurityChanges) || tempDb.AuditSecurity;
                    tempDb.AuditAdmin = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.AdminActivity) == (int)RegulationSettings.RegulationDatabaseCategory.AdminActivity) || tempDb.AuditAdmin;
                    tempDb.AuditDML = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.DatabaseModification) == (int)RegulationSettings.RegulationDatabaseCategory.DatabaseModification) || tempDb.AuditDML;
                    tempDb.AuditSELECT = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.Select) == (int)RegulationSettings.RegulationDatabaseCategory.Select) || tempDb.AuditSELECT;

                    if (((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.FilterPassedAccess) == (int)RegulationSettings.RegulationDatabaseCategory.FilterPassedAccess) || (tempDb.AuditAccessCheck == AccessCheckFilter.SuccessOnly))
                        tempDb.AuditAccessCheck = AccessCheckFilter.SuccessOnly;
                    else if (((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.FilterFailedAccess) == (int)RegulationSettings.RegulationDatabaseCategory.FilterFailedAccess) || (tempDb.AuditAccessCheck == AccessCheckFilter.FailureOnly))
                        tempDb.AuditAccessCheck = AccessCheckFilter.FailureOnly;
                    else
                        tempDb.AuditAccessCheck = AccessCheckFilter.NoFilter;

                    tempDb.AuditCaptureSQL = (CoreConstants.AllowCaptureSql && ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.SQLText) == (int)RegulationSettings.RegulationDatabaseCategory.SQLText)) || tempDb.AuditCaptureSQL;
                    tempDb.AuditCaptureTrans = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.Transactions) == (int)RegulationSettings.RegulationDatabaseCategory.Transactions) || tempDb.AuditCaptureTrans;
                    tempDb.AuditSensitiveColumns = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.SensitiveColumns) == (int)RegulationSettings.RegulationDatabaseCategory.SensitiveColumns) || tempDb.AuditSensitiveColumns;
                    tempDb.AuditDataChanges = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.BeforeAfterDataChange) == (int)RegulationSettings.RegulationDatabaseCategory.BeforeAfterDataChange) || tempDb.AuditDataChanges;

                    _privUserDbCheck = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.PrivelegedUsers) == (int)RegulationSettings.RegulationDatabaseCategory.PrivelegedUsers) || _privUserDbCheck;

                }
            }

            if (checkCIS.Checked)
            {

                if (!(checkPCI.Checked || checkHIPAA.Checked || checkDISA.Checked || checkNERC.Checked || checkSOX.Checked
                    || checkFERPA.Checked || checkGDPR.Checked || checkCustom.Checked))
                {
                    tempDb.AuditDDL = true;
                    tempDb.AuditSecurity = true;
                    tempDb.AuditAdmin = true;
                    tempDb.AuditAccessCheck = AccessCheckFilter.SuccessOnly;
                }
            }

            if (checkSOX.Checked)
            {
                if (_regulationSettings.TryGetValue((int)Regulation.RegulationType.SOX, out settings))
                {
                    tempDb.AuditDDL = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.DatabaseDefinition) == (int)RegulationSettings.RegulationDatabaseCategory.DatabaseDefinition) || tempDb.AuditDDL;
                    tempDb.AuditSecurity = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.SecurityChanges) == (int)RegulationSettings.RegulationDatabaseCategory.SecurityChanges) || tempDb.AuditSecurity;
                    tempDb.AuditAdmin = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.AdminActivity) == (int)RegulationSettings.RegulationDatabaseCategory.AdminActivity) || tempDb.AuditAdmin;
                    tempDb.AuditDML = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.DatabaseModification) == (int)RegulationSettings.RegulationDatabaseCategory.DatabaseModification) || tempDb.AuditDML;
                    tempDb.AuditSELECT = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.Select) == (int)RegulationSettings.RegulationDatabaseCategory.Select) || tempDb.AuditSELECT;

                    if (((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.FilterPassedAccess) == (int)RegulationSettings.RegulationDatabaseCategory.FilterPassedAccess) || (tempDb.AuditAccessCheck == AccessCheckFilter.SuccessOnly))
                        tempDb.AuditAccessCheck = AccessCheckFilter.SuccessOnly;
                    else if (((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.FilterFailedAccess) == (int)RegulationSettings.RegulationDatabaseCategory.FilterFailedAccess) || (tempDb.AuditAccessCheck == AccessCheckFilter.FailureOnly))
                        tempDb.AuditAccessCheck = AccessCheckFilter.FailureOnly;
                    else
                        tempDb.AuditAccessCheck = AccessCheckFilter.NoFilter;

                    tempDb.AuditCaptureSQL = (CoreConstants.AllowCaptureSql && ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.SQLText) == (int)RegulationSettings.RegulationDatabaseCategory.SQLText)) || tempDb.AuditCaptureSQL;
                    tempDb.AuditCaptureTrans = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.Transactions) == (int)RegulationSettings.RegulationDatabaseCategory.Transactions) || tempDb.AuditCaptureTrans;
                    tempDb.AuditSensitiveColumns = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.SensitiveColumns) == (int)RegulationSettings.RegulationDatabaseCategory.SensitiveColumns) || tempDb.AuditSensitiveColumns;
                    tempDb.AuditDataChanges = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.BeforeAfterDataChange) == (int)RegulationSettings.RegulationDatabaseCategory.BeforeAfterDataChange) || tempDb.AuditDataChanges;

                    _privUserDbCheck = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.PrivelegedUsers) == (int)RegulationSettings.RegulationDatabaseCategory.PrivelegedUsers) || _privUserDbCheck;

                }
            }

            if (checkFERPA.Checked)
            {
                if (_regulationSettings.TryGetValue((int)Regulation.RegulationType.FERPA, out settings))
                {
                    tempDb.AuditDDL = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.DatabaseDefinition) == (int)RegulationSettings.RegulationDatabaseCategory.DatabaseDefinition) || tempDb.AuditDDL;
                    tempDb.AuditSecurity = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.SecurityChanges) == (int)RegulationSettings.RegulationDatabaseCategory.SecurityChanges) || tempDb.AuditSecurity;
                    tempDb.AuditAdmin = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.AdminActivity) == (int)RegulationSettings.RegulationDatabaseCategory.AdminActivity) || tempDb.AuditAdmin;
                    tempDb.AuditDML = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.DatabaseModification) == (int)RegulationSettings.RegulationDatabaseCategory.DatabaseModification) || tempDb.AuditDML;
                    tempDb.AuditSELECT = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.Select) == (int)RegulationSettings.RegulationDatabaseCategory.Select) || tempDb.AuditSELECT;

                    if (((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.FilterPassedAccess) == (int)RegulationSettings.RegulationDatabaseCategory.FilterPassedAccess) || (tempDb.AuditAccessCheck == AccessCheckFilter.SuccessOnly))
                        tempDb.AuditAccessCheck = AccessCheckFilter.SuccessOnly;
                    else if (((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.FilterFailedAccess) == (int)RegulationSettings.RegulationDatabaseCategory.FilterFailedAccess) || (tempDb.AuditAccessCheck == AccessCheckFilter.FailureOnly))
                        tempDb.AuditAccessCheck = AccessCheckFilter.FailureOnly;
                    else
                        tempDb.AuditAccessCheck = AccessCheckFilter.NoFilter;

                    tempDb.AuditCaptureSQL = (CoreConstants.AllowCaptureSql && ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.SQLText) == (int)RegulationSettings.RegulationDatabaseCategory.SQLText)) || tempDb.AuditCaptureSQL;
                    tempDb.AuditCaptureTrans = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.Transactions) == (int)RegulationSettings.RegulationDatabaseCategory.Transactions) || tempDb.AuditCaptureTrans;
                    tempDb.AuditSensitiveColumns = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.SensitiveColumns) == (int)RegulationSettings.RegulationDatabaseCategory.SensitiveColumns) || tempDb.AuditSensitiveColumns;
                    tempDb.AuditDataChanges = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.BeforeAfterDataChange) == (int)RegulationSettings.RegulationDatabaseCategory.BeforeAfterDataChange) || tempDb.AuditDataChanges;

                    _privUserDbCheck = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.PrivelegedUsers) == (int)RegulationSettings.RegulationDatabaseCategory.PrivelegedUsers) || _privUserDbCheck;

                }
            }
            if (checkGDPR.Checked)
            {
                if (_regulationSettings.TryGetValue((int)Regulation.RegulationType.GDPR, out settings))
                {
                    tempDb.AuditDDL = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.DatabaseDefinition) == (int)RegulationSettings.RegulationDatabaseCategory.DatabaseDefinition) || tempDb.AuditDDL;
                    tempDb.AuditSecurity = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.SecurityChanges) == (int)RegulationSettings.RegulationDatabaseCategory.SecurityChanges) || tempDb.AuditSecurity;
                    tempDb.AuditAdmin = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.AdminActivity) == (int)RegulationSettings.RegulationDatabaseCategory.AdminActivity) || tempDb.AuditAdmin;
                    tempDb.AuditDML = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.DatabaseModification) == (int)RegulationSettings.RegulationDatabaseCategory.DatabaseModification) || tempDb.AuditDML;
                    tempDb.AuditSELECT = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.Select) == (int)RegulationSettings.RegulationDatabaseCategory.Select) || tempDb.AuditSELECT;

                    if (((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.FilterPassedAccess) == (int)RegulationSettings.RegulationDatabaseCategory.FilterPassedAccess) || (tempDb.AuditAccessCheck == AccessCheckFilter.SuccessOnly))
                        tempDb.AuditAccessCheck = AccessCheckFilter.SuccessOnly;
                    else if (((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.FilterFailedAccess) == (int)RegulationSettings.RegulationDatabaseCategory.FilterFailedAccess) || (tempDb.AuditAccessCheck == AccessCheckFilter.FailureOnly))
                        tempDb.AuditAccessCheck = AccessCheckFilter.FailureOnly;
                    else
                        tempDb.AuditAccessCheck = AccessCheckFilter.NoFilter;

                    tempDb.AuditCaptureSQL = (CoreConstants.AllowCaptureSql && ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.SQLText) == (int)RegulationSettings.RegulationDatabaseCategory.SQLText)) || tempDb.AuditCaptureSQL;
                    tempDb.AuditCaptureTrans = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.Transactions) == (int)RegulationSettings.RegulationDatabaseCategory.Transactions) || tempDb.AuditCaptureTrans;
                    tempDb.AuditSensitiveColumns = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.SensitiveColumns) == (int)RegulationSettings.RegulationDatabaseCategory.SensitiveColumns) || tempDb.AuditSensitiveColumns;
                    tempDb.AuditDataChanges = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.BeforeAfterDataChange) == (int)RegulationSettings.RegulationDatabaseCategory.BeforeAfterDataChange) || tempDb.AuditDataChanges;

                    _privUserDbCheck = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.PrivelegedUsers) == (int)RegulationSettings.RegulationDatabaseCategory.PrivelegedUsers) || _privUserDbCheck;
                }
            }

            var dbRecord = this._databases != null && _databases.Count > 0 ? _databases[0] : null;

            if (!_chkAuditDDL.Checked && tempDb.AuditDDL)
            {
                _chkAuditDDL.Checked = tempDb.AuditDDL;
                DeselectionManager.GreyOutCheckboxControls(_chkAuditDDL.Checked, GetConfigBasedCheckBox(this.chkUserAuditDDLDatabase, this.chkUserAuditDDL));
            }
            if (!_chkAuditSecurityChanges.Checked && tempDb.AuditSecurity)
            {
                _chkAuditSecurityChanges.Checked = tempDb.AuditSecurity;
                DeselectionManager.GreyOutCheckboxControls(_chkAuditSecurityChanges.Checked, GetConfigBasedCheckBox(this.chkUserAuditSecurityDatabase, this.chkUserAuditSecurity));
            }
            if (!_chkAuditAdminActivity.Checked && tempDb.AuditAdmin)
            {
                _chkAuditAdminActivity.Checked = tempDb.AuditAdmin;
                DeselectionManager.GreyOutCheckboxControls(_chkAuditAdminActivity.Checked, GetConfigBasedCheckBox(this.chkUserAuditAdminDatabase, chkUserAuditAdmin));
            }
            if (!_chkAuditDML.Checked && tempDb.AuditDML)
            {
                _chkAuditDML.Checked = tempDb.AuditDML;
                DeselectionManager.GreyOutCheckboxControls(_chkAuditDML.Checked, GetConfigBasedCheckBox(this.chkUserAuditDMLDatabase, chkUserAuditDML));
            }
            if (!_chkAuditSelect.Checked && tempDb.AuditSELECT)
            {
                _chkAuditSelect.Checked = tempDb.AuditSELECT;
                DeselectionManager.GreyOutCheckboxControls(_chkAuditSelect.Checked, GetConfigBasedCheckBox(this.chkUserAuditSELECTDatabase, chkUserAuditSELECT));
            }

            if (_server.AuditAccessCheck == AccessCheckFilter.NoFilter && (dbRecord == null || dbRecord.AuditAccessCheck == AccessCheckFilter.NoFilter) && _chkDBAccessCheckFilter.Enabled)
            {
                if (tempDb.AuditAccessCheck == AccessCheckFilter.NoFilter)
                {
                    _chkDBAccessCheckFilter.Enabled = true;
                    _chkDBAccessCheckFilter.Checked = false;
                    rbDBAuditFailedOnly.Enabled = false;
                    rbDBAuditSuccessfulOnly.Enabled = false;
                }
                else
                {
                    _chkDBAccessCheckFilter.Enabled = true;
                    _chkDBAccessCheckFilter.Checked = true;
                    rbDBAuditFailedOnly.Enabled = true;
                    rbDBAuditSuccessfulOnly.Enabled = true;
                    if (tempDb.AuditAccessCheck == AccessCheckFilter.FailureOnly)
                    {
                        rbDBAuditFailedOnly.Checked = true;
                    }
                    else
                    {
                        rbDBAuditSuccessfulOnly.Checked = true;
                    }
                }
                if (_server.AuditUserAccessCheck == AccessCheckFilter.NoFilter)
                {
                    // Update Dependent Controls
                    EnableCb(
                        GetConfigBasedCheckBox(this.chkUserAccessCheckFilterDatabase, chkUserAccessCheckFilter),
                        _chkDBAccessCheckFilter.Enabled);
                    CheckCb(
                        GetConfigBasedCheckBox(this.chkUserAccessCheckFilterDatabase, chkUserAccessCheckFilter),
                        _chkDBAccessCheckFilter.Checked);
                    EnableRb(
                        GetConfigBasedRadioButton(this.rbUserAuditSuccessfulOnlyDatabase, rbUserAuditSuccessfulOnly),
                        rbDBAuditSuccessfulOnly.Enabled);
                    EnableRb(
                        GetConfigBasedRadioButton(this.rbUserAuditFailedOnlyDatabase, rbUserAuditFailedOnly),
                        rbDBAuditFailedOnly.Enabled);
                    CheckRb(
                        GetConfigBasedRadioButton(this.rbUserAuditSuccessfulOnlyDatabase, rbUserAuditSuccessfulOnly),
                        rbDBAuditSuccessfulOnly.Checked);
                    CheckRb(
                        GetConfigBasedRadioButton(this.rbUserAuditFailedOnlyDatabase, rbUserAuditFailedOnly),
                        rbDBAuditFailedOnly.Checked);
                }
            }

            // Disable sql capture if DML and Select both unchecked
            if ((tempDb.AuditDML || tempDb.AuditSELECT || _chkAuditDML.Checked || _chkAuditSelect.Checked)
                && CoreConstants.AllowCaptureSql && !(_chkCaptureSQL.Checked && !_chkCaptureSQL.Enabled))
            {
                _chkCaptureSQL.Enabled = true;
            }
            else
            {
                _chkCaptureSQL.Enabled = false;
            }

            if ((dbRecord == null || dbRecord.AuditUserDML || dbRecord.AuditUserSELECT) && CoreConstants.AllowCaptureSql
                && !_server.AuditUserCaptureSQL && !(GetConfigBasedCheckBox(this.chkUserAuditCaptureSQLDatabase, chkUserAuditCaptureSQL).Checked
                                                     && !GetConfigBasedCheckBox(this.chkUserAuditCaptureSQLDatabase, chkUserAuditCaptureSQL).Enabled))
            {
                GetConfigBasedCheckBox(this.chkUserAuditCaptureSQLDatabase, chkUserAuditCaptureSQL).Enabled = true;
            }
            else
            {
                GetConfigBasedCheckBox(this.chkUserAuditCaptureSQLDatabase, chkUserAuditCaptureSQL).Enabled = false;
            }

            if (!_chkCaptureSQL.Checked && tempDb.AuditCaptureSQL)
            {
                _chkCaptureSQL.Checked = tempDb.AuditCaptureSQL;
                DeselectionManager.GreyOutCheckboxControls(_chkCaptureSQL.Checked, GetConfigBasedCheckBox(this.chkUserAuditCaptureSQLDatabase, chkUserAuditCaptureSQL));
            }



            // Disable trans capture if DML unchecked
            if ((tempDb.AuditDML || _chkAuditDML.Checked) && ServerRecord.CompareVersions(_server.AgentVersion, "3.5") >= 0 && !(_chkCaptureTrans.Checked && !_chkCaptureTrans.Enabled))
                _chkCaptureTrans.Enabled = true;
            else
                _chkCaptureTrans.Enabled = false;

            if ((dbRecord == null || dbRecord.AuditUserDML) && ServerRecord.CompareVersions(_server.AgentVersion, "3.5") >= 0
                && !_server.AuditUserCaptureTrans && !(GetConfigBasedCheckBox(this.chkUserAuditCaptureTransDatabase, chkUserAuditCaptureTrans).Checked
                                                     && !GetConfigBasedCheckBox(this.chkUserAuditCaptureTransDatabase, chkUserAuditCaptureTrans).Enabled))
            {
                GetConfigBasedCheckBox(this.chkUserAuditCaptureTransDatabase, chkUserAuditCaptureTrans).Enabled = true;
            }
            else
            {
                GetConfigBasedCheckBox(this.chkUserAuditCaptureTransDatabase, chkUserAuditCaptureTrans).Enabled = false;
            }

            if (!_chkCaptureTrans.Checked && tempDb.AuditCaptureTrans)
            {
                _chkCaptureTrans.Checked = tempDb.AuditCaptureTrans;
                DeselectionManager.GreyOutCheckboxControls(_chkCaptureTrans.Checked, GetConfigBasedCheckBox(this.chkUserAuditCaptureTransDatabase, chkUserAuditCaptureTrans));
            }

            //Disable DDL and Security capture if DDL and Security both unchecked
            if ((tempDb.AuditDDL || tempDb.AuditSecurity || _chkAuditDDL.Checked || _chkAuditSecurityChanges.Checked) && CoreConstants.AllowCaptureSql && !(_chkDBCaptureDDL.Checked && !_chkDBCaptureDDL.Enabled))
                _chkDBCaptureDDL.Enabled = true;
            else
                _chkDBCaptureDDL.Enabled = false;

            if ((dbRecord == null || dbRecord.AuditUserDDL || dbRecord.AuditUserSecurity) && CoreConstants.AllowCaptureSql
                && !_server.AuditUserCaptureDDL && !(GetConfigBasedCheckBox(chkUserCaptureDDLDatabase, chkUserCaptureDDL).Checked
                                                     && !GetConfigBasedCheckBox(chkUserCaptureDDLDatabase, chkUserCaptureDDL).Enabled))
            {
                GetConfigBasedCheckBox(chkUserCaptureDDLDatabase, chkUserCaptureDDL).Enabled = true;
            }
            else
            {
                GetConfigBasedCheckBox(chkUserCaptureDDLDatabase, chkUserCaptureDDL).Enabled = false;
            }

            if (!_chkDBCaptureDDL.Checked && tempDb.AuditCaptureDDL)
            {
                _chkDBCaptureDDL.Checked = tempDb.AuditCaptureDDL;
                DeselectionManager.GreyOutCheckboxControls(_chkDBCaptureDDL.Checked, GetConfigBasedCheckBox(chkUserCaptureDDLDatabase, chkUserCaptureDDL));
            }

            if (!_chkAuditBeforeAfter.Checked && tempDb.AuditDataChanges)
                _chkAuditBeforeAfter.Checked = tempDb.AuditDataChanges;

            if (!_chkAuditSensitiveColumn.Checked && tempDb.AuditSensitiveColumns)
                _chkAuditSensitiveColumn.Checked = tempDb.AuditSensitiveColumns;


            _chkPriviligedUser.Visible = true;
            _chkPriviligedUser.Checked = _privUserDbCheck;
        }
        #endregion

        #region Before After
        private void InitDataChangeColumns()
        {
            TreeNode root = new TreeNode();
            TreeNode node;
            RawDatabaseObject rdo;
            root.Text = _server.Instance;
            treeBadDatabases.Nodes.Clear();
            if (_startPage == StartPage.RegulationGuidelineInfo)
            {

                foreach (DatabaseRecord db in _databases)
                {
                    node = new TreeNode();
                    rdo = new RawDatabaseObject();
                    node.Text = rdo.name = db.Name;
                    node.Tag = rdo;
                    root.Nodes.Add(node);
                    LoadDataChangeColumns(db);
                }
            }
            else
            {
                foreach (RawDatabaseObject db in listDatabases.CheckedItems)
                {
                    node = new TreeNode();
                    node.Text = db.name;
                    node.Tag = db;
                    root.Nodes.Add(node);
                }
            }
            treeBadDatabases.Nodes.Add(root);
            treeBadDatabases.ExpandAll();
            treeBadDatabases.SelectedNode = root.Nodes[0];
            _loadedBADTables = true;
        }

        private void LoadDataChangeColumns(DatabaseRecord db)
        {
            if (db.AuditDataChanges)
            {
                if (_loadedBADTables)
                    return;

                List<string> missingTables = new List<string>();
                List<DataChangeTableRecord> badTables = DataChangeTableRecord.GetAuditedTables(Globals.Repository.Connection, _server.SrvId, db.DbId);
                if (badTables.Count > 0)
                {
                    if (!LoadTables(db.Name))
                    {
                        MessageBox.Show(UIConstants.Error_CantLoadTables);
                        return;
                    }
                    _lvBeforeAfterTables = new ListView();
                    foreach (DataChangeTableRecord table in badTables)
                    {
                        ListViewItem x = _lvBeforeAfterTables.Items.Add(table.FullTableName);
                        x.SubItems.Add(Form_MaxRows.GetMaxRowString(table.RowLimit));
                        if (SupportsBeforeAfterColumns())
                        {
                            x.SubItems.Add(table.SelectedColumns ? Form_TableConfigure.GetColumnString(table.Columns) : UIConstants.BAD_AllColumns);
                        }
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
                    _badListViewItems[db.Name] = _lvBeforeAfterTables;
                }
                _tableList = _tableObjects = null;
            }
        }

        private void treeBadDatabases_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (treeBadDatabases.SelectedNode.Parent == null)
            {
                treeBadDatabases.SelectedNode = treeBadDatabases.SelectedNode.Nodes[0];
            }
            RawDatabaseObject db = (RawDatabaseObject)treeBadDatabases.SelectedNode.Tag;
            if (_startPage == StartPage.RegulationGuidelineInfo)
            {
                foreach (var dbRecord in _databases)
                {
                    if (db.name == dbRecord.Name)
                    {
                        _dbBadTreeSelection = dbRecord;
                        break;
                    }
                }
            }
            else
            {
                _dbBadTreeSelection = new DatabaseRecord();
                _dbBadTreeSelection.Name = db.name;
            }
        }

        private void treeBadDatabases_BeforeSelect(object sender, TreeViewCancelEventArgs e)
        {
            _tableList = _tableObjects = null;
            buttonConfigureBad.Enabled = true;
        }

        private void buttonConfigureBad_Click(object sender, EventArgs e)
        {
            Form_BeforeAfterRegulationGuidelines badRegulatioGuidelinesForm = new Form_BeforeAfterRegulationGuidelines(_server, _dbBadTreeSelection, _chkAuditDML.Checked, _badListViewItems);
            if (DialogResult.OK == badRegulatioGuidelinesForm.ShowDialog())
            {
                _lvBeforeAfterTables = badRegulatioGuidelinesForm.LvBeforeAfterTables;
                _badListViewItems[_dbBadTreeSelection.Name] = _lvBeforeAfterTables;
            }
        }

        private List<DataChangeTableRecord> GetBATables(string dbName)
        {
            List<DataChangeTableRecord> retVal = new List<DataChangeTableRecord>();
            ListView temp_lvBeforeAfterTables = new ListView();
            if (_badListViewItems.TryGetValue(dbName, out temp_lvBeforeAfterTables))
            {
                foreach (ListViewItem item in temp_lvBeforeAfterTables.Items)
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
            }
            return retVal;
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

        #endregion

        #region Custom Template

        private void _chkServerAccessCheckFilter_CheckedChanged(object sender, EventArgs e)
        {
            if (_templateSvrInitializationCompleted)
            {
                _isCustomTemplate = true;
            }
            if (_chkServerAccessCheckFilter.Checked)
            {
                _rbAuditFailedOnly.Enabled = true;
                _rbServerAuditSuccessfulOnly.Enabled = true;
            }
            else
            {
                _rbAuditFailedOnly.Enabled = false;
                _rbServerAuditSuccessfulOnly.Enabled = false;
            }
        }

        private void _checkLogins_CheckedChanged(object sender, EventArgs e)
        {
            if (_templateSvrInitializationCompleted)
            {
                _isCustomTemplate = true;
            }
            // SQLCM-5375-6.1.4.3-Capture Logout Events only if Logins are captured
            this._checkLogouts.Enabled = this._checkLogins.Checked;
        }

        /// <summary>
        /// Added checked changed events for Logouts
        /// </summary>
        /// <remarks>
        /// SQLCM-5375-6.1.4.3-Capture Logout Events
        /// </remarks>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _checkLogouts_CheckedChanged(object sender, EventArgs e)
        {
            if (_templateSvrInitializationCompleted)
            {
                _isCustomTemplate = true;
            }
        }

        private void _checkFailedLogins_CheckedChanged(object sender, EventArgs e)
        {
            if (_templateSvrInitializationCompleted)
            {
                _isCustomTemplate = true;
            }
        }

        private void _checkSecurityChanges_CheckedChanged(object sender, EventArgs e)
        {
            if (_templateSvrInitializationCompleted)
            {
                _isCustomTemplate = true;
            }
        }

        private void _checkDDL_CheckedChanged(object sender, EventArgs e)
        {
            if (_templateSvrInitializationCompleted)
            {
                _isCustomTemplate = true;
            }
        }

        private void _checkAdministrativeActivities_CheckedChanged(object sender, EventArgs e)
        {
            if (_templateSvrInitializationCompleted)
            {
                _isCustomTemplate = true;
            }
        }

        private void checkUserDefinedEvents_CheckedChanged(object sender, EventArgs e)
        {
            if (_templateSvrInitializationCompleted)
            {
                _isCustomTemplate = true;
            }
        }

        private void checkPriviligedUser_CheckedChanged(object sender, EventArgs e)
        {
            if (_templateSvrInitializationCompleted)
            {
                _isCustomTemplate = true;
            }
        }

        private void _rbServerAuditSuccessfulOnly_CheckedChanged(object sender, EventArgs e)
        {
            if (_templateSvrInitializationCompleted)
            {
                _isCustomTemplate = true;
            }
        }

        private void _rbAuditFailedOnly_CheckedChanged(object sender, EventArgs e)
        {
            if (_templateSvrInitializationCompleted)
            {
                _isCustomTemplate = true;
            }
        }

        private void checkCustom_CheckedChanged(object sender, EventArgs e)
        {
            UpdateNext();
            if (checkCustom.Checked)
            {
                _tbFile.Enabled = true;
                _btnBrowse.Enabled = true;
                _isCustomTemplate = true;
            }
            else
            {
                _tbFile.Enabled = false;
                _btnBrowse.Enabled = false;
                _isCustomTemplate = false;
            }
        }

        private void UpdateServerRecord(AuditCategory cat, ServerRecord tempServer)
        {
            switch (cat)
            {
                case AuditCategory.Logins:
                    tempServer.AuditLogins = true;
                    break;
                case AuditCategory.Logouts:
                    tempServer.AuditLogouts = true;
                    break;
                case AuditCategory.DDL:
                    tempServer.AuditDDL = true;
                    break;
                case AuditCategory.Security:
                    tempServer.AuditSecurity = true;
                    break;
                case AuditCategory.Admin:
                    tempServer.AuditAdmin = true;
                    break;
                case AuditCategory.FailedLogins:
                    tempServer.AuditFailedLogins = true;
                    break;
                case AuditCategory.UDC: // User defined
                    tempServer.AuditUDE = true;
                    break;
            }
        }

        private void UpdateDBRecord(DatabaseRecord tempDb, DBAuditConfig config)
        {
            // Basic DB Settings
            foreach (AuditCategory cat in config.Categories)
            {
                switch (cat)
                {
                    case AuditCategory.DDL:
                        tempDb.AuditDDL = true;
                        break;
                    case AuditCategory.Security:
                        tempDb.AuditSecurity = true;
                        break;
                    case AuditCategory.Admin:
                        tempDb.AuditAdmin = true;
                        break;
                    case AuditCategory.SELECT:
                        tempDb.AuditSELECT = true;
                        break;
                    case AuditCategory.DML:
                        tempDb.AuditDML = true;
                        break;
                    case AuditCategory.UDC:
                        tempDb.AuditDataChanges = true;
                        break;

                }
            }
            tempDb.AuditCaptureSQL = config.KeepSQL;
            tempDb.AuditCaptureTrans = config.AuditTrans;
            tempDb.AuditCaptureDDL = config.AuditDDL;
            tempDb.AuditAccessCheck = (AccessCheckFilter)config.AccessCheckFilter;
        }

        private void UpdateUserRecord(UserAuditConfig _userConfig, ServerRecord record)
        {
            foreach (AuditCategory cat in _userConfig.Categories)
            {
                switch (cat)
                {
                    case AuditCategory.Logins:
                        record.AuditUserLogins = true;
                        break;
                    case AuditCategory.Logouts:
                        record.AuditUserLogouts = true;
                        break;
                    case AuditCategory.DDL:
                        record.AuditUserDDL = true;
                        break;
                    case AuditCategory.Security:
                        record.AuditUserSecurity = true;
                        break;
                    case AuditCategory.Admin:
                        record.AuditUserAdmin = true;
                        break;
                    case AuditCategory.FailedLogins:
                        record.AuditUserFailedLogins = true;
                        break;
                    case AuditCategory.UDC: // User defined
                        record.AuditUserUDE = true;
                        break;
                    case AuditCategory.SELECT:
                        record.AuditUserSELECT = true;
                        break;
                    case AuditCategory.DML:
                        record.AuditUserDML = true;
                        break;
                }
            }

            record.AuditUserCaptureSQL = record.AuditUserCaptureSQL || _userConfig.KeepSQL;
            record.AuditUserCaptureTrans = record.AuditUserCaptureTrans || _userConfig.CaptureTrans;
            record.AuditUserCaptureDDL = record.AuditUserCaptureDDL || _userConfig.CaptureDDL;
            record.AuditUserAccessCheck = (AccessCheckFilter)_userConfig.AccessCheckFilter;
        }

        private void UpdateDBUserRecord(UserAuditConfig _userConfig, DatabaseRecord db)
        {
            foreach (AuditCategory cat in _userConfig.Categories)
            {
                switch (cat)
                {
                    case AuditCategory.Logins:
                        db.AuditUserLogins = true;
                        break;
                    case AuditCategory.Logouts:
                        db.AuditUserLogouts = true;
                        break;
                    case AuditCategory.DDL:
                        db.AuditUserDDL = true;
                        break;
                    case AuditCategory.Security:
                        db.AuditUserSecurity = true;
                        break;
                    case AuditCategory.Admin:
                        db.AuditUserAdmin = true;
                        break;
                    case AuditCategory.FailedLogins:
                        db.AuditUserFailedLogins = true;
                        break;
                    case AuditCategory.UDC: // User defined
                        db.AuditUserUDE = true;
                        break;
                    case AuditCategory.SELECT:
                        db.AuditUserSELECT = true;
                        break;
                    case AuditCategory.DML:
                        db.AuditUserDML = true;
                        break;
                }
            }

            db.AuditUserCaptureSQL = db.AuditUserCaptureSQL || _userConfig.KeepSQL;
            db.AuditUserCaptureTrans = db.AuditUserCaptureTrans || _userConfig.CaptureTrans;
            db.AuditUserCaptureDDL = db.AuditUserCaptureDDL || _userConfig.CaptureDDL;
            db.AuditUserAccessCheck = (AccessCheckFilter)_userConfig.AccessCheckFilter;
        }

        private void _chkDBAccessCheckFilter_CheckedChanged(object sender, EventArgs e)
        {
            if (_templateDBInitializationCompleted)
            {
                _isCustomTemplate = true;
            }
            if (_chkDBAccessCheckFilter.Checked)
            {
                _rbDBAuditFailedOnly.Enabled = true;
                _rbDBAuditSuccessfulOnly.Enabled = true;
            }
            else
            {
                _rbDBAuditFailedOnly.Enabled = false;
                _rbDBAuditSuccessfulOnly.Enabled = false;
            }
        }

        private void _rbDBAuditFailedOnly_CheckedChanged(object sender, EventArgs e)
        {
            if (_templateDBInitializationCompleted)
            {
                _isCustomTemplate = true;
            }
        }

        private void _rbDBAuditSuccessfulOnly_CheckedChanged(object sender, EventArgs e)
        {
            if (_templateDBInitializationCompleted)
            {
                _isCustomTemplate = true;
            }
        }

        private void _chkCaptureSQL_CheckedChanged(object sender, EventArgs e)
        {
            if (_templateDBInitializationCompleted)
            {
                _isCustomTemplate = true;
            }
            if (_chkCaptureSQL.Checked && _isCustomTemplate)
            {
                if (!_templateSvrInitializationCompleted || !this._templateDBInitializationCompleted)
                {
                    return;
                }
                ErrorMessage.Show(this.Text, UIConstants.Warning_CaptureAll, "", MessageBoxIcon.Warning);
            }
        }

        private void _chkCaptureTrans_CheckedChanged(object sender, EventArgs e)
        {
            if (_templateDBInitializationCompleted)
            {
                _isCustomTemplate = true;
            }
        }

        private void _chkDBCaptureDDL_CheckedChanged(object sender, EventArgs e)
        {
            if (_templateDBInitializationCompleted)
            {
                _isCustomTemplate = true;
            }
        }

        private void _chkAuditDDL_CheckedChanged(object sender, EventArgs e)
        {
            if (_templateDBInitializationCompleted)
            {
                _isCustomTemplate = true;
            }
            if ((_chkAuditDDL.Checked || _chkAuditSecurityChanges.Checked) && CoreConstants.AllowCaptureSql)
                _chkDBCaptureDDL.Enabled = true;
            else
                _chkDBCaptureDDL.Enabled = false;
        }

        private void _chkAuditSecurityChanges_CheckedChanged(object sender, EventArgs e)
        {
            if (_templateDBInitializationCompleted)
            {
                _isCustomTemplate = true;
            }
            if ((_chkAuditDDL.Checked || _chkAuditSecurityChanges.Checked) && CoreConstants.AllowCaptureSql)
                _chkDBCaptureDDL.Enabled = true;
            else
                _chkDBCaptureDDL.Enabled = false;
        }

        private void _chkAuditAdminActivity_CheckedChanged(object sender, EventArgs e)
        {
            if (_templateDBInitializationCompleted)
            {
                _isCustomTemplate = true;
            }
        }

        private void _chkAuditSensitiveColumn_CheckedChanged(object sender, EventArgs e)
        {
            if (_templateDBInitializationCompleted)
            {
                _isCustomTemplate = true;
            }
        }

        private void _chkAuditBeforeAfter_CheckedChanged(object sender, EventArgs e)
        {
            if (_templateDBInitializationCompleted)
            {
                _isCustomTemplate = true;
            }
        }

        private void _chkPriviligedUser_CheckedChanged(object sender, EventArgs e)
        {
            if (_templateDBInitializationCompleted)
            {
                _isCustomTemplate = true;
            }
        }

        private void _chkAuditDML_CheckedChanged(object sender, EventArgs e)
        {
            if (_templateDBInitializationCompleted)
            {
                _isCustomTemplate = true;
            }

            if ((_chkAuditDML.Checked || _chkAuditSelect.Checked) && CoreConstants.AllowCaptureSql)
                _chkCaptureSQL.Enabled = true;
            else
                _chkCaptureSQL.Enabled = false;

            if (_chkAuditDML.Checked && ServerRecord.CompareVersions(_server.AgentVersion, "3.5") >= 0)
                _chkCaptureTrans.Enabled = true;
            else
            {
                _chkCaptureTrans.Enabled = false;
                _chkCaptureTrans.Checked = false;
            }
        }

        #endregion

        #region SaveDialog

        public void ExportServerAuditSettingsAction(ServerRecord server, string fileName)
        {
            if (server != null)
            {
                SaveFileDialog dlg = new SaveFileDialog();
                dlg.Filter = "XML files (*.xml)|*.xml|All files (*.*)|*.*";
                dlg.FilterIndex = 1;
                dlg.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                dlg.FileName = fileName;
                dlg.Title = "Export Regulation Guideline Audit Settings";

                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    //Dump the data
                    InstanceTemplate tmpl = new InstanceTemplate();
                    tmpl.RepositoryServer = Globals.RepositoryServer;
                    tmpl.ImportAuditSettings(server.Instance);
                    tmpl.Save(dlg.FileName);
                }
            }
        }

        public void ExportDatabaseAuditSettingsAction(List<DatabaseRecord> db, string fileName)
        {
            if (db != null)
            {
                SaveFileDialog dlg = new SaveFileDialog();
                dlg.Filter = "XML files (*.xml)|*.xml|All files (*.*)|*.*";
                dlg.FilterIndex = 1;
                dlg.InitialDirectory = string.IsNullOrEmpty(m_currentExportingDirectory)
                     ? Environment.GetFolderPath(Environment.SpecialFolder.Personal) : m_currentExportingDirectory;
                dlg.Title = "Export Database Audit Settings";
                dlg.FileName = fileName;
                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    m_currentExportingDirectory = Path.GetDirectoryName(dlg.FileName);
                    //Dump the data
                    for (int i = 0; i < db.Count; i++)
                    {
                        InstanceTemplate tmpl = new InstanceTemplate();
                        tmpl.RepositoryServer = Globals.RepositoryServer;
                        tmpl.ExportDatabaseAuditSettings(db[i]);
                        string fileExtension = Path.GetExtension(dlg.FileName);
                        fileName = dlg.FileName.Replace(fileExtension, "_" + db[i].Name + fileExtension);
                        tmpl.Save(fileName);
                    }
                }
            }
        }

        #endregion

        #region Configure
        private void button_Configure_Click(object sender, EventArgs e)
        {
            Form_ConfigureSensitiveColumn configureSensitiveColumnsForm = new Form_ConfigureSensitiveColumn(_server, _dbSCTreeSelection, _dictLvSCTables);
            if (DialogResult.OK == configureSensitiveColumnsForm.ShowDialog())
            {
                _lvSCTables = configureSensitiveColumnsForm.LvSCTables;

                //Add current listview to dictionary
                _dictLvSCTables[_dbSCTreeSelection.Name] = _lvSCTables;

            }
        }

        private IList LoadColumns(string tableName)
        {
            IList columnList = null;
            // Load list of columns for the table
            // try via connection to agent
            if (_server.IsDeployed && _server.IsRunning)
            {
                string url = "";
                try
                {
                    AgentManager manager = GUIRemoteObjectsProvider.AgentManager();

                    columnList = manager.GetRawColumns(_server.Instance, _dbName, tableName);
                }
                catch (Exception ex)
                {
                    ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                            String.Format("LoadColumns: URL: {0} Instance {1} Database {2} Table {3}", url, _server.Instance, _dbName, tableName),
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
                    columnList = RawSQL.GetColumns(_sqlServer.Connection, _dbName, tableName);
                }
            }

            return columnList;
        }
        #endregion

        private void LinkedLabelClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {

            this.linkLabel1.LinkVisited = true;

            System.Diagnostics.Process.Start("http://wiki.idera.com/x/xwI1");

        }

        private void infoToolTip_MouseEnter(object sender, EventArgs e)
        {
            if (((PictureBox)sender).Name == pictureCISInfo.Name)
            {
                toolTipCIS.Visible = true;
            }
            else if (((PictureBox)sender).Name == pictureDISAInfo.Name)
            {
                toolTipDISA.Visible = true;
            }
            else if (((PictureBox)sender).Name == pictureHIPPAInfo.Name)
            {
                toolTipHIPAA.Visible = true;
            }
            else if (((PictureBox)sender).Name == pictureNERCInfo.Name)
            {
                toolTipNERC.Visible = true;
            }
            else if (((PictureBox)sender).Name == picturePCIInfo.Name)
            {
                toolTipPCI.Visible = true;
            }
            else if (((PictureBox)sender).Name == pictureSOXInfo.Name)
            {
                toolTipSOX.Visible = true;
            }
            else if (((PictureBox)sender).Name == pictureFERPAInfo.Name)
            {
                toolTipFERPA.Visible = true;
            }
            else if (((PictureBox)sender).Name == pictureGDPRInfo.Name)
            {
                toolTipGDPR.Visible = true;
            }
        }

        private void inforToolTip_MouseLeave(object sender, EventArgs e)
        {
            if (((PictureBox)sender).Name == pictureCISInfo.Name)
            {
                toolTipCIS.Visible = false;
            }
            else if (((PictureBox)sender).Name == pictureDISAInfo.Name)
            {
                toolTipDISA.Visible = false;
            }
            else if (((PictureBox)sender).Name == pictureHIPPAInfo.Name)
            {
                toolTipHIPAA.Visible = false;
            }
            else if (((PictureBox)sender).Name == pictureNERCInfo.Name)
            {
                toolTipNERC.Visible = false;
            }
            else if (((PictureBox)sender).Name == picturePCIInfo.Name)
            {
                toolTipPCI.Visible = false;
            }
            else if (((PictureBox)sender).Name == pictureSOXInfo.Name)
            {
                toolTipSOX.Visible = false;
            }
            else if (((PictureBox)sender).Name == pictureFERPAInfo.Name)
            {
                toolTipFERPA.Visible = false;
            }
            else if (((PictureBox)sender).Name == pictureGDPRInfo.Name)
            {
                toolTipGDPR.Visible = false;
            }
        }
    }
}
