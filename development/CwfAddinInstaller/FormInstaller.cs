using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using SQLcomplianceCwfAddin.RestService;
//using Idera.SQLcompliance.Core.Cwf;

namespace CwfAddinInstaller
{
    public partial class FormInstaller : Form
    {
        #region members

        private string _cancelMessage;
        private Dictionary<WizardPage, KeyValuePair<WizardPageInfo, WizardPageBase>> _wizardPages;
        private WizardPage _currentPage;
        public readonly StringBuilder _logs;
        private bool _isPreviousEnabled;
        private bool _isNextEnabled;
        private bool _isCancelEnabled;

        private const int CpNocloseButton = 0x200;
        internal const string NoPassowrdlogging = "<we will not log this>";

        
        internal delegate void AppendLog(string message, params object[] parameters);
        internal event AppendLog OnLogAppended;

        #endregion

        #region constructor \ destructor

        public FormInstaller()
        {
            _logs = new StringBuilder();
            CwfAddInZip = Path.Combine(Application.StartupPath, RestServiceConstants.ZipFile);
            CwfDashboardInstaller = Path.Combine(Application.StartupPath, "IderaDashboard.msi");
            MainApplicationInstaller = Path.Combine(Application.StartupPath, Environment.Is64BitOperatingSystem ? "SQLcompliance-x64.exe" : "SQLcompliance.exe"); ;
            CwfHelper = new CwfHelper(this);

            InitializeComponent();

            _isNextEnabled = btnNext.Enabled;
        }

        ~FormInstaller()
        {
            _wizardPages.Clear();
            _logs.Clear();
        }

        #endregion

        #region properties

        internal CwfHelper CwfHelper { get; private set; }

        internal string CwfAddInZip { get; private set; }

        internal string CwfDashboardInstaller { get; private set; }

        internal string MainApplicationInstaller { get; private set; }

        protected override CreateParams CreateParams
        {
            get
            {
                var creationParams = base.CreateParams;
                creationParams.ClassStyle = creationParams.ClassStyle | CpNocloseButton;
                return creationParams;
            }
        }

        #endregion

        #region internal \ public methods

        internal void Log(string message, params object[] parameters)
        {
            _logs.AppendFormat(message, parameters);
            _logs.AppendLine();

            if (OnLogAppended != null)
                OnLogAppended(message, parameters);
        }

        internal void AllowNext(bool isAllowed)
        {
            btnNext.Enabled = isAllowed;
            _isNextEnabled = isAllowed;
        }

        internal void AllowCancel(bool isAllowed)
        {
            btnCancel.Enabled = isAllowed;
            _isCancelEnabled = isAllowed;
        }

        internal void ChangeNextButtonText(string text)
        {
            btnNext.Text = text;
        }

        internal void SetHeader()
        {
            lblPageTitle.BringToFront();
        }

        internal void ChangeCancelButtonText(string text)
        {
            btnCancel.Text = text;
        }

        internal bool IsInputValid(Control control)
        {
            if ((control is TextBox || 
                control is ComboBox) && 
                string.IsNullOrEmpty(control.Text))
            {
                control.Focus();
                return false;
            }

            return true;
        }

        internal DialogResult ShowError(string message, params object[] parameters)
        {
            return MessageBox.Show(this, string.Format(message, parameters), Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        #endregion

        private void InitializeWizardPages()
        {
            _currentPage = WizardPage.NotSpecified;

            _wizardPages = new Dictionary<WizardPage, KeyValuePair<WizardPageInfo, WizardPageBase>>();
            foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (!type.IsClass)
                    continue;

                var attributes = type.GetCustomAttributes(typeof (WizardPageInfo), false);
                if (attributes.Length != 1)
                    continue;

                var wizardPageInfo = (WizardPageInfo) attributes[0];
                var wizardPageEntry = new KeyValuePair<WizardPageInfo, WizardPageBase>(wizardPageInfo, (WizardPageBase) Activator.CreateInstance(type, this));
                _wizardPages.Add(wizardPageInfo.Page, wizardPageEntry);
            }
        }

        private void FormInstaller_Load(object sender, EventArgs e)
        {
            _cancelMessage = string.Format("Are you sure you want to cancel {0}?", Text);
            InitializeWizardPages();
            NavigateToPage(NavigationDirection.Next);
        }

        #region wizard navigation

        private void AllowNavigation(bool isAllowed)
        {
            btnNext.Enabled = _isNextEnabled;
            btnPrevious.Enabled = _isPreviousEnabled;
            btnCancel.Enabled = _isCancelEnabled;
            Cursor = isAllowed ? Cursors.Default : Cursors.WaitCursor;
            Application.DoEvents();
        }

        /// <summary>
        /// This method asks the current page to perform its operation and return success or failure.
        /// </summary>
        private bool DoAction()
        {
            if (!_wizardPages.ContainsKey(_currentPage)) 
                return true;

            _isPreviousEnabled = btnPrevious.Enabled;
            _isCancelEnabled = btnCancel.Enabled;
            AllowNavigation(false);

            var currentPage = _wizardPages[_currentPage];

            string errorMessage;
            Control invalidControl;
            var operationSuccessful = currentPage.Value.DoAction(out errorMessage, out invalidControl);
            if (!operationSuccessful)
            {
                ShowError(errorMessage);
                if (invalidControl != null)
                    invalidControl.Focus();
            }

            // if install fails, we give user chance to retry or cancel setup
            if (!operationSuccessful && currentPage.Key.Page == WizardPage.Install)
            {
                AllowCancel(true);
                AllowNext(true);
            }

            AllowNavigation(true);

            return operationSuccessful;
        }

        internal void NavigateToPage(NavigationDirection direction)
        {
            // don't navigate forward if current page's operation fails
            if (_currentPage != WizardPage.ConfiguringSQLCM)
            {
                if (direction == NavigationDirection.Next && !DoAction())
                    return;
            }

            var nextPage = direction == NavigationDirection.Next ?
                           _currentPage + 1 : direction == NavigationDirection.Previous ?
                           _currentPage - 1 : _currentPage + (WizardPage.Error - _currentPage);

            if (direction == NavigationDirection.Finish)
            {
                nextPage = _currentPage + (WizardPage.Finish - _currentPage);
            }

            if (!_wizardPages.ContainsKey(nextPage))
                return;

            var wizardPage = _wizardPages[nextPage];
            _currentPage = nextPage;

            switch (_currentPage)
            {
                case WizardPage.Welcome:
                    btnPrevious.Visible = false;
                    btnNext.Text = @"Install";
                    break;

                case WizardPage.DashboardLocation:
                    btnPrevious.Visible = false;
                    btnCancel.Enabled = true;
                    break;

                case WizardPage.ReadyToInstall:
                    btnPrevious.Enabled = true;
                    btnCancel.Enabled = true;
                    btnNext.Text = @"Install";
                    break;

                case WizardPage.Install:
                    btnNext.Text = @"Next";
                    btnPrevious.Enabled = false;
                    btnCancel.Enabled = false;
                    break;

                case WizardPage.Finish:
                    btnPrevious.Visible = false;
                    btnCancel.Enabled = false;
                    btnNext.Text = @"Finish";
                    break;

                case WizardPage.ConfiguringSQLCM:
                    btnNext.Text = @"Next";
                    btnPrevious.Enabled = false;
                    btnCancel.Enabled = false;
                    break;

                case WizardPage.ConfiguringCWFDashboard:
                    btnNext.Text = @"Next";
                    btnPrevious.Enabled = false;
                    btnCancel.Enabled = false;
                    break;

                case WizardPage.Error:
                    btnNext.Text = @"Finish";
                    btnPrevious.Enabled = false;
                    btnCancel.Enabled = false;
                    btnPrevious.Visible = false;
                    break;

                default:
                    btnNext.Text = @"Next";
                    btnCancel.Enabled = true;
                    btnPrevious.Visible = true;
                    btnPrevious.Enabled = true;
                    break;
            }

            AllowNext(true);
            if (_currentPage == WizardPage.ConfiguringSQLCM || _currentPage == WizardPage.ConfiguringCWFDashboard)
            {
                AllowNext(false);
            }

            lblPageTitle.Text = wizardPage.Key.Title;
            lblPageDescription.Text = wizardPage.Key.Description;
            pnlPageHost.Controls.Clear();
            pnlPageHost.Controls.Add(wizardPage.Value);

            if (!wizardPage.Value.IsInitialized)
                wizardPage.Value.Initialize();

            _isPreviousEnabled = btnPrevious.Enabled;
            _isCancelEnabled = btnCancel.Enabled;
            AllowNavigation(false);

            wizardPage.Value.OnNavigated(direction);
            wizardPage.Value.Focus();

            AllowNavigation(true);
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (_currentPage == WizardPage.Finish)
            {
                // don't navigate forward if current page's operation fails
                if (!DoAction())
                    return;

                DialogResult = DialogResult.OK;
                Close();
                Application.Exit();
            }
            else
                NavigateToPage(NavigationDirection.Next);
        }

        private void btnPrevious_Click(object sender, EventArgs e)
        {
            if (_currentPage == WizardPage.Welcome)
                return;

            NavigateToPage(NavigationDirection.Previous);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (_currentPage == WizardPage.Install)
            {

                Text = "Failed SQLCM Product Registration";
                _cancelMessage = "IDERA SQL Compliance Manager & Dashboard Installation has completed. However, registering SQL Compliance Manager to IDERA Dashboard has failed. If you exit the installation, you will have to register SQL Compliance Manager manually to the IDERA Dashboard by going to Manage Products screen in IDERA Dashboard Administration. Are you sure you want to exit?";
                if (MessageBox.Show(this, _cancelMessage, Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                {
                    Text = "IDERA SQL Compliance Manager and Dashboard Setup";
                    return;
                }
            }
            else
            {
                if (MessageBox.Show(this, _cancelMessage, Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                    return;
            }

            DialogResult = DialogResult.Cancel;
            CloseInstaller();
        }

        #endregion

        private void FormInstaller_Shown(object sender, EventArgs e)
        {
            // skip file check during debugging
            if (Debugger.IsAttached)
                return;

            if (!File.Exists(CwfDashboardInstaller))
            {
                ShowError("SQL Compliance Manager dashboard installer file '{0}' is missing.", CwfDashboardInstaller);
                CloseInstaller();
                return;
            }

            if (!File.Exists(MainApplicationInstaller))
            {
                ShowError("SQL Compliance Manager installer file '{0}' is missing.", MainApplicationInstaller);
                CloseInstaller();
                return;
            }

            if (!File.Exists(CwfAddInZip))
            {
                ShowError("SQL Compliance Manager CWF Add-In zip file '{0}' is missing.", CwfAddInZip);
                CloseInstaller();
            }
        }

        public void CloseInstaller()
        {
            Close();
            Application.Exit();
        }

        private void FormInstaller_FormClosing(object sender, FormClosingEventArgs e)
        {
            Log("Closing {0}.", Text);

            try
            {
                var logFile = Path.Combine(Application.StartupPath, "SQLCM_CWF_AddIn_Installer_Log.txt");
                File.WriteAllText(logFile, _logs.ToString());
            }
            catch (Exception ex)
            {
                ShowError("Failed to save installation logs. Error: {0}", ex);
            }
        }
    }
}
