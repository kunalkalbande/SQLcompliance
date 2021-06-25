using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using SQLCM_Installer.Custom_Controls;

namespace SQLCM_Installer.WizardPages
{
    [WizardPageInfo(WizardPage.InstallationDirectory)]
    internal partial class PageInstallationDirectory : WizardPageBase
    {
        public static bool isDashboardAvailable;

        public PageInstallationDirectory(MainForm host)
            : base(host)
        {
            InitializeComponent();
            this.toolTipDisplayName.BringToFront();
            this.toolTipRemoteDashboard.BringToFront();
        }

        internal override void Initialize()
        {
            IsInitialized = true;
        }

        private void cmInstallDirBrowseButton_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderDlg = new FolderBrowserDialog();
            folderDlg.ShowNewFolderButton = true;
            DialogResult result = folderDlg.ShowDialog();

            if (result == DialogResult.OK)
            {
                txtCMInstallDir.Text = folderDlg.SelectedPath;
                Environment.SpecialFolder root = folderDlg.RootFolder;
            }
        }

        private void dashboardInstallDirBrowseButton_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderDlg = new FolderBrowserDialog();
            folderDlg.ShowNewFolderButton = true;
            DialogResult result = folderDlg.ShowDialog();

            if (result == DialogResult.OK)
            {
                txtDashboardInstallDir.Text = folderDlg.SelectedPath;
                Environment.SpecialFolder root = folderDlg.RootFolder;
            }
        }

        internal override void OnNavigated(NavigationDirection direction)
        {
            labelRemoteDashboardCred.Disabled = true;
            labelRemoteDashboardUserName.Disabled = true;
            labelRemoteDashboardPassword.Disabled = true;
            SetPageControlsAsSetupType();
        }

        internal override bool DoAction(out string errorMessage, out Control invalidControl)
        {
            errorMessage = null;
            invalidControl = null;

            #region DirectoryValidate
            string rootUserDrive;
            if (Constants.UserInstallType != InstallType.DashboardOnly)
            {
            InstallProperties.CMInstallDir = txtCMInstallDir.Text;
                Constants.SetDirectoryList(txtCMInstallDir.Name,InstallProperties.CMInstallDir);
            if (string.IsNullOrEmpty(InstallProperties.CMInstallDir))
            {
                errorMessage = Constants.CMInstallDirEmptyError;
                return false;
            }
            rootUserDrive = Path.GetPathRoot(InstallProperties.CMInstallDir);
            if (!Directory.Exists(rootUserDrive))
            {
                errorMessage = Constants.CMInstallDirError;
                return false;
            }
            else
            {
                try
                {
                    Directory.CreateDirectory(Path.Combine(InstallProperties.CMInstallDir, "TempDir"));
                    Directory.Delete(Path.Combine(InstallProperties.CMInstallDir, "TempDir"));
                }
                catch
                {
                    errorMessage = Constants.CMInstallDirError;
                    return false;
                }
            }
            }
            if (Constants.UserInstallType == InstallType.DashboardOnly
                || Constants.UserInstallType == InstallType.AgentAndDashboard
                || Constants.UserInstallType == InstallType.ConsoleAndDashboard
                || Constants.UserInstallType == InstallType.CMAndDashboard)
            {
                if (radioDashboardLocal.Checked)
                {
                    InstallProperties.DashboardInstallDir = txtDashboardInstallDir.Text;
                    Constants.SetDirectoryList(txtDashboardInstallDir.Name, InstallProperties.DashboardInstallDir);
                    if (string.IsNullOrEmpty(InstallProperties.DashboardInstallDir))
                    {
                        errorMessage = Constants.DashboardInstallDirEmptyError;
                        return false;
                    }
                    rootUserDrive = Path.GetPathRoot(InstallProperties.DashboardInstallDir);
                    if (!Directory.Exists(rootUserDrive))
                    {
                        errorMessage = Constants.DashboardInstallDirError;
                        return false;
                    }
                    else
                    {
                        if (!Directory.Exists(InstallProperties.DashboardInstallDir))
                        {
                        try
                        {
                            Directory.CreateDirectory(Path.Combine(InstallProperties.DashboardInstallDir, "TempDir"));
                                Directory.Delete(InstallProperties.DashboardInstallDir, true);
                        }
                        catch
                        {
                            errorMessage = Constants.DashboardInstallDirError;
                            return false;
                            }
                        }
                    }
                }
            }
            #endregion

            #region DisplayNameValidate

            if (Constants.UserInstallType == InstallType.CMAndDashboard
                || Constants.UserInstallType == InstallType.CMOnly && InstallProperties.RegisterProductToDashboard)
            {
                InstallProperties.CMDisplayName = txtDisplayName.Text;
                Host.CwfHelper.CwfProductInstance = InstallProperties.CMDisplayName;
                if (InstallProperties.CMDisplayName.Equals(String.Empty))
                {
                    errorMessage = Constants.InvalidDisplayNameError;
                    return false;
                }

                for (var index = 0; index < InstallProperties.CMDisplayName.Length; index++)
                {
                    if (char.IsLetterOrDigit(InstallProperties.CMDisplayName, index))
                        continue;

                    if (InstallProperties.CMDisplayName[index].Equals('-'))
                        continue;

                    errorMessage = Constants.InvalidDisplayNameError;
                    return false;
                }
            }

            if (InstallProperties.RegisterProductToDashboard)
            {
                List<string> productList = Host.CwfHelper.GetRegisteredSQLCMProductNames();
                if (productList != null && productList.Count > 0)
                {
                    if (productList.Contains(InstallProperties.CMDisplayName))
                    {
                        errorMessage = string.Format("Another product is already registered with '{0}' display name with same IDERA Dashboard. Please specify unique display name.", InstallProperties.CMDisplayName);
                        return false;
                    }
                }
            }
            #endregion

            #region RemoteDashboardCredValidate
            if (Constants.UserInstallType == InstallType.CMAndDashboard && radioDashboardRemote.Checked)
            {
                if (string.IsNullOrEmpty(txtRemoteDashboardUrl.Text))
                {
                    errorMessage = Constants.DashboardURLEmpty;
                    return false;
                }
                if (string.IsNullOrEmpty(txtRemoteDashboardUserName.Text))
                {
                    errorMessage = Constants.DashboardUserInvalid;
                    return false;
                }
                if (string.IsNullOrEmpty(txtRemoteDashboardPassword.Text))
                {
                    errorMessage = Constants.DashboardPasswordInvalid;
                    return false;
                }

                Uri inputURL;
                if (!Uri.TryCreate(txtRemoteDashboardUrl.Text, UriKind.RelativeOrAbsolute, out inputURL))
                {
                    errorMessage = Constants.DashboardUriInvalid;
                    return false;
                }
                try
                {
                Host.CwfHelper.CwfUrl = string.Format("http://{0}:{1}", inputURL.Host, inputURL.Port);
                }
                catch
                {
                    errorMessage = Constants.DashboardUriInvalid;
                    return false;
                }
                Host.CwfHelper.CwfUser = txtRemoteDashboardUserName.Text;
                Host.CwfHelper.CwfPassword = txtRemoteDashboardPassword.Text;

                isDashboardAvailable = Host.CwfHelper.CanConnectToDashboard();

                if (isDashboardAvailable)
                {
                    Version installedDashboardVersion = Host.CwfHelper.GetCurrentVersion(out errorMessage);

                    if (installedDashboardVersion == null &&
                        !string.IsNullOrEmpty(errorMessage))
                    {
                        errorMessage = string.Format("Couldn't get current version of dashboard due to the following error: {0}", errorMessage);
                        return false;
                    }

                    if (installedDashboardVersion < Constants.DashboardVersion)
                    {
                        errorMessage = string.Format(Constants.DashboardVersionError, inputURL.Host);
                        return false;
                    }

                    List<string> productList = Host.CwfHelper.GetRegisteredSQLCMProductNames();
                    if (productList != null && productList.Count > 0)
                    {
                        if (productList.Contains(InstallProperties.CMDisplayName))
                        {
                            errorMessage = string.Format("Another product is already registered with '{0}' display name with same IDERA Dashboard. Please specify unique display name.", InstallProperties.CMDisplayName);
                            return false;
                        }
                    }
                }
                else
                {
                    Host.CwfHelper.CwfUser = "";
                    Host.CwfHelper.CwfPassword = "";
                    errorMessage = Constants.InvalidRemoteDashboardValues;
                    return false;
                }

            }

            #endregion

            return true;
        }

        public void SetPageControlsAsSetupType()
        {
            if (Constants.UserInstallType == InstallType.DashboardOnly)
            {
                radioDashboardLocal.HideRadio(true);
            }
            else
            {
                radioDashboardLocal.HideRadio(false);
            }

            if (Constants.UserInstallType == InstallType.DashboardOnly)
            {
                panelCMInstallDir.Visible = false;
                panelDashboardHeader.Visible = true;
                panelDashboardInstallDir.Visible = true;
                panelDisplayname.Visible = false;
                panelDashboardHeader.Location = new Point(0, 64);
                panelDashboardInstallDir.Location = new Point(0, 110);
                radioDashboardRemote.Visible = false;
                txtRemoteDashboardUrl.Visible = false;
                labelRemoteDashboardCred.Visible = false;
                labelRemoteDashboardUserName.Visible = false;
                labelRemoteDashboardPassword.Visible = false;
                txtRemoteDashboardUserName.Visible = false;
                txtRemoteDashboardPassword.Visible = false;
                remoteDashboardInfoPicture.Visible = false;
                labelSubHeader.Text = Constants.InstallDashboardOnlyInstallDirScreen;
            }
            if (Constants.UserInstallType == InstallType.CMOnly
                || Constants.UserInstallType == InstallType.AgentOnly
                || Constants.UserInstallType == InstallType.ConsoleOnly)
            {
                panelCMInstallDir.Visible = true;
                panelDashboardHeader.Visible = false;
                panelDashboardInstallDir.Visible = false;
                if (InstallProperties.RegisterProductToDashboard)
                {
                    panelDisplayname.Visible = true;
                }
                else
                {
                    panelDisplayname.Visible = false;
                }
                panelDashboardHeader.Location = new Point(0, 200);
                panelDashboardInstallDir.Location = new Point(0, 238);
                radioDashboardRemote.Visible = false;
                txtRemoteDashboardUrl.Visible = false;
                labelRemoteDashboardCred.Visible = false;
                labelRemoteDashboardUserName.Visible = false;
                labelRemoteDashboardPassword.Visible = false;
                txtRemoteDashboardUserName.Visible = false;
                txtRemoteDashboardPassword.Visible = false;
                remoteDashboardInfoPicture.Visible = false;
                if (Constants.UserInstallType == InstallType.CMOnly)
                {
                    labelSubHeader.Text = Constants.InstallCMOnlyInstallDirScreen;
                    labelSQLCMHeader.Text = Constants.ProductMap[Products.Compliance];
                }
                else if (Constants.UserInstallType == InstallType.AgentOnly)
                {
                    labelSubHeader.Text = Constants.InstallAgentOnlyInstallDirScreen;
                    labelSQLCMHeader.Text = Constants.ProductMap[Products.Agent];
                }
                else if (Constants.UserInstallType == InstallType.ConsoleOnly)
                {
                    labelSubHeader.Text = Constants.InstallConsoleOnlyInstallDirScreen;
                    labelSQLCMHeader.Text = Constants.ProductMap[Products.Console];
                }
            }
            if (Constants.UserInstallType == InstallType.CMAndDashboard)
            {
                panelCMInstallDir.Visible = true;
                panelDashboardHeader.Visible = true;
                panelDashboardInstallDir.Visible = true;
                panelDisplayname.Visible = true;
                panelDashboardHeader.Location = new Point(0, 200);
                panelDashboardInstallDir.Location = new Point(0, 238);
                radioDashboardRemote.Visible = true;
                txtRemoteDashboardUrl.Visible = true;
                labelRemoteDashboardCred.Visible = true;
                labelRemoteDashboardUserName.Visible = true;
                labelRemoteDashboardPassword.Visible = true;
                txtRemoteDashboardUserName.Visible = true;
                txtRemoteDashboardPassword.Visible = true;
                remoteDashboardInfoPicture.Visible = true;

                labelRemoteDashboardCred.Location = new Point(40, 137);
                labelRemoteDashboardUserName.Location = new Point(40, 164);
                labelRemoteDashboardPassword.Location = new Point(40, 198);
                txtRemoteDashboardUserName.Location = new Point(202, 162);
                txtRemoteDashboardPassword.Location = new Point(202, 199);

                labelSubHeader.Text = Constants.InstallCMAndDashboardInstallDirScreen;
                labelSQLCMHeader.Text = Constants.ProductMap[Products.Compliance];
            }
            if (Constants.UserInstallType == InstallType.AgentAndDashboard
                || Constants.UserInstallType == InstallType.ConsoleAndDashboard)
            {
                panelCMInstallDir.Visible = true;
                panelDashboardHeader.Visible = true;
                panelDashboardInstallDir.Visible = true;
                panelDisplayname.Visible = false;
                panelDashboardHeader.Location = new Point(0, 150);
                panelDashboardInstallDir.Location = new Point(0, 188);
                radioDashboardRemote.Visible = false;
                txtRemoteDashboardUrl.Visible = false;
                labelRemoteDashboardCred.Visible = false;
                labelRemoteDashboardUserName.Visible = false;
                labelRemoteDashboardPassword.Visible = false;
                txtRemoteDashboardUserName.Visible = false;
                txtRemoteDashboardPassword.Visible = false;

                remoteDashboardInfoPicture.Visible = false;
                if (Constants.UserInstallType == InstallType.AgentAndDashboard)
                {
                    labelSubHeader.Text = Constants.InstallAgentAndDashboardInstallDirScreen;
                    labelSQLCMHeader.Text = Constants.ProductMap[Products.Agent];
                }
                else
                {
                    labelSubHeader.Text = Constants.InstallConsoleAndDashboardInstallDirScreen;
                    labelSQLCMHeader.Text = Constants.ProductMap[Products.Console];
                }
            }
        }

        private void radioDashboardLocal_Click(object sender, EventArgs e)
        {
            if (radioDashboardLocal.Checked)
            {
                txtDashboardInstallDir.Enabled = true;
                dashboardInstallDirBrowseButton.Disabled = false;
                txtRemoteDashboardUrl.Enabled = false;
                labelRemoteDashboardCred.Disabled = true;
                labelRemoteDashboardUserName.Disabled = true;
                labelRemoteDashboardPassword.Disabled = true;
                txtRemoteDashboardUserName.Enabled = false;
                txtRemoteDashboardPassword.Enabled = false;
            }
        }

        private void radioDashboardRemote_Click(object sender, EventArgs e)
        {
            if (radioDashboardRemote.Checked)
            {
                txtDashboardInstallDir.Enabled = false;
                dashboardInstallDirBrowseButton.Disabled = true;
                txtRemoteDashboardUrl.Enabled = true;
                labelRemoteDashboardCred.Disabled = false;
                labelRemoteDashboardUserName.Disabled = false;
                labelRemoteDashboardPassword.Disabled = false;
                txtRemoteDashboardUserName.Enabled = true;
                txtRemoteDashboardPassword.Enabled = true;
                IderaMessageBox messageBox = new IderaMessageBox();
                messageBox.Show(Constants.RemoteDashboardWarningMessage, "Information");
            }
        }

        private void CMDisplayNameInfoPicture_MouseEnter(object sender, EventArgs e)
        {
            toolTipDisplayName.Visible = true;
        }

        private void CMDisplayNameInfoPicture_MouseLeave(object sender, EventArgs e)
        {
            toolTipDisplayName.Visible = false;
        }

        private void remoteDashboardInfoPicture_MouseEnter(object sender, EventArgs e)
        {
            toolTipRemoteDashboard.Visible = true;
        }

        private void remoteDashboardInfoPicture_MouseLeave(object sender, EventArgs e)
        {
            toolTipRemoteDashboard.Visible = false;
        }
    }
}
