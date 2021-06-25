using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SQLCM_Installer.Custom_Prompts;
using Microsoft.Win32;
using SQLCM_Installer.Custom_Controls;

namespace SQLCM_Installer.WizardPages
{
    [WizardPageInfo(WizardPage.Repositories)]
    internal partial class PageRepositories : WizardPageBase
    {
        List<string> localSQLServerInstanceList;
        bool localCall = false;

        public PageRepositories(MainForm host)
            : base(host)
        {
            InitializeComponent();
        }

        internal override void Initialize()
        {
            IsInitialized = true;
        }

        internal override void OnNavigated(NavigationDirection direction)
        {
            string failureMessage = string.Empty;
            if (InstallProperties.IsUpgradeRadioSelection)
            {
                if (Constants.UserInstallType == InstallType.CMOnly || Constants.UserInstallType == InstallType.CMAndDashboard)
                {
                    SetControlsForUpgrade();
                    GetSQLServerInstanceFromCMRegistry();
                    dropdownCMSQLServerInstance.RemoveItems();
                    dropdownCMSQLServerInstance.SetItems(localSQLServerInstanceList);
                    dropdownCMSQLServerInstance.SetDefaultItem(localSQLServerInstanceList[0]);
                    txtCMUser.Text = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                    InstallProperties.CMSQLServerInstanceName = dropdownCMSQLServerInstance.SelectedItem;
                }
            }
            else
            {
                if (string.IsNullOrEmpty(InstallProperties.CMSQLServerInstanceName) || string.IsNullOrEmpty(InstallProperties.DashboardSQLServerInstanceName))
                {
                    txtCMUser.Text = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                    txtDashboardUser.Text = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                    this.AutoScroll = true;
                    if (this.VerticalScroll.Visible)
                    {
                        this.AutoScrollPosition = new Point(0, 0);
                        this.VerticalScroll.Value = 0;
                    }

                    Host.SetCMandDashboardCredOk(false, false);
                    SetPageControlsAsSetupType();
                    GetLocalSQLServerInstanceList();
                    SetPanelSize();
                    dropdownCMSQLServerInstance.RemoveItems();
                    dropdownDashboardSQLServerInstance.RemoveItems();
                    dropdownCMSQLServerInstance.SetItems(localSQLServerInstanceList);
                    dropdownDashboardSQLServerInstance.SetItems(localSQLServerInstanceList);
                    if (localSQLServerInstanceList.Count == 0 && (!InstallProperties.Clustered))
                    {
                        failureMessage = "No SQL Server Instances have been detected on this local machine. SQL Compliance Manager requires a SQL Server Instance to install the repository. Please run the installation on a machine with a valid SQL Server Instance.";
                        Host.Invoke(new MethodInvoker(delegate
                        {
                            Host.ShowError(failureMessage);
                        }));
                    }
                    else
                    {
                        dropdownCMSQLServerInstance.SetDefaultItem(localSQLServerInstanceList[0]);
                        dropdownDashboardSQLServerInstance.SetDefaultItem(localSQLServerInstanceList[0]);
                        InstallProperties.CMSQLServerInstanceName = dropdownCMSQLServerInstance.SelectedItem;
                        InstallProperties.DashboardSQLServerInstanceName = dropdownDashboardSQLServerInstance.SelectedItem;
                    }
                }
                else if (!string.IsNullOrEmpty(InstallProperties.CMSQLServerInstanceName))
                {
                    this.AutoScroll = true;
                    if (this.VerticalScroll.Visible)
                    {
                        this.AutoScrollPosition = new Point(0, 0);
                        this.VerticalScroll.Value = 0;
                    }

                    Host.SetCMandDashboardCredOk(false, false);
                    SetPageControlsAsSetupType();
                    GetLocalSQLServerInstanceList();
                    if (localSQLServerInstanceList.Count == 0 && (!InstallProperties.Clustered))
                    {
                        failureMessage = "No SQL Server Instances have been detected on this local machine. SQL Compliance Manager requires a SQL Server Instance to install the repository. Please run the installation on a machine with a valid SQL Server Instance.";
                        Host.Invoke(new MethodInvoker(delegate
                        {
                            Host.ShowError(failureMessage);
                        }));
                    }
                    SetPanelSize();
                    dropdownCMSQLServerInstance.SetDefaultItem(InstallProperties.CMSQLServerInstanceName);
                }
                else if (!string.IsNullOrEmpty(InstallProperties.DashboardSQLServerInstanceName))
                {
                    this.AutoScroll = true;
                    if (this.VerticalScroll.Visible)
                    {
                        this.AutoScrollPosition = new Point(0, 0);
                        this.VerticalScroll.Value = 0;
                    }

                    Host.SetCMandDashboardCredOk(false, false);
                    SetPageControlsAsSetupType();
                    SetPanelSize();
                    dropdownDashboardSQLServerInstance.SetDefaultItem(InstallProperties.DashboardSQLServerInstanceName);
                }
            }
        }

        private void SetPanelSize()
        {
            if (this.checkBoxClusteredEnv.Checked && this.VerticalScroll.Visible)
            {
                panelSubHeaderPanel.Width = 533;
                panelSQLCMRepository.Width = 533;
                panelSQLCMRepositoryHeader.Width = 533;
                panelDashboardRepositoryHeader.Width = 533;
            }
            else
            {
                panelSubHeaderPanel.Width = 550;
                panelSQLCMRepository.Width = 550;
                panelSQLCMRepositoryHeader.Width = 550;
                panelDashboardRepositoryHeader.Width = 550;
            }
        }

        public void GetLocalSQLServerInstanceList()
        {
            localSQLServerInstanceList = new List<string>();
            string fullSQLServerInstance = "";

            RegistryView registryView = Environment.Is64BitOperatingSystem ? RegistryView.Registry64 : RegistryView.Registry32;
            using (RegistryKey hklm = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, registryView))
            {
                RegistryKey instanceKey = hklm.OpenSubKey(@"SOFTWARE\Microsoft\Microsoft SQL Server\Instance Names\SQL", false);
                if (instanceKey != null)
                {
                    foreach (var instanceName in instanceKey.GetValueNames())
                    {
                        if (!instanceName.Equals("MSSQLSERVER"))
                        {
                            fullSQLServerInstance = Environment.MachineName + "\\" + instanceName;
                        }
                        else
                        {
                            fullSQLServerInstance = Environment.MachineName;
                        }
                        localSQLServerInstanceList.Add(fullSQLServerInstance);
                    }
                }
            }
        }

        public void SetPageControlsAsSetupType()
        {
            if (Constants.UserInstallType == InstallType.CMOnly)
            {
                panelDashboardRepositoryHeader.Visible = false;
                panelDashboardRepository.Visible = false;
                labelSubHeader.Text = Constants.InstallCMAndDashboardRespositoryScreen;
                panelClusteredEnv.Visible = true;
                panelSQLCMRepositoryHeader.Visible = true;
                panelSQLCMRepository.Visible = true;
            }
            if (Constants.UserInstallType == InstallType.CMAndDashboard)
            {
                panelDashboardRepositoryHeader.Visible = true;
                panelDashboardRepository.Visible = true;
                panelDashboardRepository.Location = new Point(0, 330);
                if (checkBoxClusteredEnv.Checked)
                {
                    panelDashboardRepository.Location = new Point(0, 370);
                }
                if (this.checkBoxSameCredAsCM.Checked)
                {
                    this.dropdownDashboardSQLServerInstance.Disabled = true;
                    this.radioDashboardSQLServerAuth.Disabled = true;
                    this.txtDashboardUser.Disabled = true;
                    this.radioDashboardWindowAuth.Disabled = true;
                    this.labelDashboardDatabaseName.Disabled = true;
                    this.labelDashboardDatabaseNameValue.Disabled = true;
                    this.labelDashboardSQLServerInstance.Disabled = true;
                }
                labelSubHeader.Text = Constants.InstallCMAndDashboardRespositoryScreen;
                panelClusteredEnv.Visible = true;
                panelSQLCMRepositoryHeader.Visible = true;
                panelSQLCMRepository.Visible = true;
                panelDashboardRepositoryHeader.Visible = true;
            }
            if (Constants.UserInstallType == InstallType.ConsoleAndDashboard
                || Constants.UserInstallType == InstallType.AgentAndDashboard
                || Constants.UserInstallType == InstallType.DashboardOnly)
            {
                panelDashboardRepository.Location = new Point(0, 64);
                labelSubHeader.Text = Constants.InstallDashboardOnlyRepositoryScreen;
                panelClusteredEnv.Visible = false;
                panelSQLCMRepositoryHeader.Visible = false;
                panelSQLCMRepository.Visible = false;
                panelDashboardRepositoryHeader.Visible = false;
                panelDashboardRepository.Visible = true;
                this.dropdownDashboardSQLServerInstance.Disabled = false;
                this.radioDashboardSQLServerAuth.Disabled = false;
                this.txtDashboardUser.Disabled = false;
                this.radioDashboardWindowAuth.Disabled = false;
                this.labelDashboardDatabaseName.Disabled = false;
                this.labelDashboardDatabaseNameValue.Disabled = false;
                this.labelDashboardSQLServerInstance.Disabled = false;
            }
        }

        private void checkBoxSameCredAsCM_Click(object sender, EventArgs e)
        {
            if (checkBoxSameCredAsCM.Checked)
            {
                localCall = true;
                this.radioDashboardWindowAuth.Checked = this.radioCMWindowAuth.Checked;
                this.radioDashboardSQLServerAuth.Checked = this.radioCMSQLServerAuth.Checked;
                this.dropdownDashboardSQLServerInstance.Disabled = true;
                this.radioDashboardSQLServerAuth.Disabled = true;
                this.txtDashboardUser.Disabled = true;
                this.radioDashboardWindowAuth.Disabled = true;
                this.labelDashboardDatabaseName.Disabled = true;
                this.labelDashboardDatabaseNameValue.Disabled = true;
                this.labelDashboardSQLServerInstance.Disabled = true;
                this.dropdownDashboardSQLServerInstance.SetDefaultItem(this.dropdownCMSQLServerInstance.SelectedItem);

                txtDashboardSQLServerUser.Visible = txtCMSQLServerUser.Visible;
                txtDashboardSQLServerUser.Text = txtCMSQLServerUser.Text;
                linkLabelChangeDashboardCred.Visible = linkLabelChangeCMCred.Visible;
                txtDashboardUser.Visible = txtCMUser.Visible;
                linkLabelChangeDashboardCred.Enabled = false;
                txtDashboardSQLServerUser.Disabled = true;
                InstallProperties.DashboardSQLServerUsername = InstallProperties.CMSQLServerUsername;
                InstallProperties.DashboardSQLServerPassword = InstallProperties.CMSQLServerPassword;
                localCall = false;
            }
            else
            {
                this.dropdownDashboardSQLServerInstance.Disabled = false;
                this.dropdownDashboardSQLServerInstance.Disabled = false;
                this.radioDashboardSQLServerAuth.Disabled = false;
                this.txtDashboardUser.Disabled = false;
                this.radioDashboardWindowAuth.Disabled = false;
                this.labelDashboardDatabaseName.Disabled = false;
                this.labelDashboardDatabaseNameValue.Disabled = false;
                this.dropdownDashboardSQLServerInstance.Disabled = false;
                this.labelDashboardSQLServerInstance.Disabled = false;
                linkLabelChangeDashboardCred.Enabled = true;
                txtDashboardSQLServerUser.Disabled = false;
            }
        }

        private void checkBoxClusteredEnv_Click(object sender, EventArgs e)
        {
            if (checkBoxClusteredEnv.Checked)
            {
                this.AutoScroll = true;
                panelNode.Visible = true;
                panelNode.Location = new Point(0, 105);
                panelSQLCMRepository.Location = new Point(0, 143);
                panelDashboardRepositoryHeader.Location = new Point(0, 332);
                panelDashboardRepository.Location = new Point(0, 370);

            }
            else
            {
                this.AutoScroll = false;
                panelNode.Visible = false;
                panelSQLCMRepository.Location = new Point(0, 112);
                panelDashboardRepositoryHeader.Location = new Point(0, 301);
                panelDashboardRepository.Location = new Point(0, 341);
                this.AutoScrollPosition = new Point(0, 0);
                this.VerticalScroll.Value = 0;
            }
            SetPanelSize();
        }

        private void radioCMSQLServerAuth_Click(object sender, EventArgs e)
        {
            if (radioCMSQLServerAuth.Checked)
            {
                FormCMSQLServerCred formCMSQLServerCred = new FormCMSQLServerCred();
                formCMSQLServerCred.ShowDialog();
                if (InstallProperties.CMSQLServerUsername.Equals(string.Empty) && InstallProperties.CMSQLServerPassword.Equals(string.Empty))
                {
                    txtCMUser.Visible = false;
                    radioCMWindowAuth.Checked = true;
                    radioCMSQLServerAuth.Checked = false;
                }
                else
                {
                    txtCMSQLServerUser.Visible = true;
                    txtCMSQLServerUser.Text = InstallProperties.CMSQLServerUsername;
                    linkLabelChangeCMCred.Visible = true;
                    txtCMUser.Visible = false;
                }
            }
            else
            {
                txtCMUser.Visible = true;
                txtCMSQLServerUser.Visible = false;
                linkLabelChangeCMCred.Visible = false;
                InstallProperties.IsCMSQLServerSQLAuth = false;
                InstallProperties.CMSQLServerUsername = "";
                InstallProperties.CMSQLServerPassword = "";
            }

            if (checkBoxSameCredAsCM.Checked)
            {
                localCall = true;
                radioDashboardSQLServerAuth.Disabled = false;
                radioDashboardWindowAuth.Disabled = false;
                if (radioCMSQLServerAuth.Checked)
                    radioDashboardSQLServerAuth.Checked = true;
                else if (radioCMWindowAuth.Checked)
                    radioDashboardWindowAuth.Checked = true;
                radioDashboardSQLServerAuth.Disabled = true;
                radioDashboardWindowAuth.Disabled = true;
                InstallProperties.IsDashboardSQLServerSQLAuth = InstallProperties.IsCMSQLServerSQLAuth;
                InstallProperties.DashboardSQLServerUsername = InstallProperties.CMSQLServerUsername;
                InstallProperties.DashboardSQLServerPassword = InstallProperties.CMSQLServerPassword;
                txtDashboardSQLServerUser.Visible = txtCMSQLServerUser.Visible;
                linkLabelChangeDashboardCred.Visible = linkLabelChangeCMCred.Visible;
                txtDashboardUser.Visible = txtCMUser.Visible;
                txtDashboardSQLServerUser.Text = txtCMSQLServerUser.Text;
                localCall = false;
            }
        }

        private void radioDashboardSQLServerAuth_Click(object sender, EventArgs e)
        {
            if (localCall)
            {
                return;
            }
            if (radioDashboardSQLServerAuth.Checked)
            {
                FormDashboardSQLServerCred formDashboardSQLServerCred = new FormDashboardSQLServerCred();
                formDashboardSQLServerCred.ShowDialog();
                if (InstallProperties.DashboardSQLServerUsername.Equals(string.Empty) && InstallProperties.DashboardSQLServerPassword.Equals(string.Empty))
                {
                    txtDashboardUser.Visible = false;
                    radioDashboardWindowAuth.Checked = true;
                    radioDashboardSQLServerAuth.Checked = false;
                }
                else
                {
                    txtDashboardSQLServerUser.Visible = true;
                    txtDashboardSQLServerUser.Text = InstallProperties.DashboardSQLServerUsername;
                    linkLabelChangeDashboardCred.Visible = true;
                    txtDashboardUser.Visible = false;
                }
            }
            else
            {
                txtDashboardUser.Visible = true;
                txtDashboardSQLServerUser.Visible = false;
                linkLabelChangeDashboardCred.Visible = false;
                InstallProperties.IsDashboardSQLServerSQLAuth = false;
                InstallProperties.DashboardSQLServerUsername = "";
                InstallProperties.DashboardSQLServerPassword = "";
            }
        }

        internal override bool DoAction(out string errorMessage, out Control invalidControl)
        {
            errorMessage = null;
            invalidControl = null;

            if (checkBoxClusteredEnv.Checked)
            {
                InstallProperties.Clustered = true;
                if (radioActiveNode.Checked)
                {
                    InstallProperties.IsActiveNode = true;
                }
                else if (radioPassiveNode.Checked)
                {
                    InstallProperties.IsActiveNode = false;
                }
            }
            else
            {
                InstallProperties.Clustered = false;
                InstallProperties.IsActiveNode = false;
            }

            if (radioCMWindowAuth.Checked)
            {
                InstallProperties.IsCMSQLServerSQLAuth = false;
            }
            else if (radioCMSQLServerAuth.Checked)
            {
                InstallProperties.IsCMSQLServerSQLAuth = true;
            }

            InstallProperties.CMSQLServerInstanceName = dropdownCMSQLServerInstance.SelectedItem;

            //Jira -5613
            if (!InstallProperties.Clustered)
            {
                if (localSQLServerInstanceList.Count == 0)
                {
                    errorMessage = "No SQL Server Instances have been detected on this local machine. SQL Compliance Manager requires a SQL Server Instance to install the repository. Please run the installation on a machine with a valid SQL Server Instance.";
                    this.Cursor = Cursors.Default;
                    return false;
                }
                else if (!localSQLServerInstanceList.Contains(InstallProperties.CMSQLServerInstanceName))
                {
                    errorMessage = "You have entered a server name that is not available on the local machine. Please choose a SQL Server Instance that is available on your local machine.";
                    this.Cursor = Cursors.Default;
                    return false;
                }
            }

            if (checkBoxSameCredAsCM.Checked)
            {
                InstallProperties.IsDashboardSQLServerSQLAuth = InstallProperties.IsCMSQLServerSQLAuth;
                InstallProperties.DashboardSQLServerInstanceName = InstallProperties.CMSQLServerInstanceName;
                InstallProperties.DashboardSQLServerUsername = InstallProperties.CMSQLServerUsername;
                InstallProperties.DashboardSQLServerPassword = InstallProperties.CMSQLServerPassword;
            }
            else
            {
                InstallProperties.DashboardSQLServerInstanceName = dropdownDashboardSQLServerInstance.SelectedItem;
                if (radioDashboardWindowAuth.Checked)
                {
                    InstallProperties.IsDashboardSQLServerSQLAuth = false;
                }
                else if (radioDashboardSQLServerAuth.Checked)
                {
                    InstallProperties.IsDashboardSQLServerSQLAuth = true;
                }
            }

            this.Cursor = Cursors.WaitCursor;
            if (!ValidateCredentials(out errorMessage))
            {
                this.Cursor = Cursors.Default;
                return false;
            }
            HelperFunctions helperFunction = new HelperFunctions();
            if (!InstallProperties.IsUpgradeRadioSelection && (!InstallProperties.Clustered || InstallProperties.IsActiveNode))
            {
                if (Constants.UserInstallType == InstallType.CMAndDashboard || Constants.UserInstallType == InstallType.CMOnly)
                {
                    InstallProperties.UpgradeSchema = false;
                    if (helperFunction.IsSQLComplianceDBAvialable(InstallProperties.IsCMSQLServerSQLAuth, InstallProperties.CMSQLServerInstanceName, InstallProperties.CMSQLServerUsername, InstallProperties.CMSQLServerPassword, out errorMessage))
                    {
                        IderaMessageBoxWithWarning ideraWarningMessageBox = new IderaMessageBoxWithWarning();
                        ideraWarningMessageBox.Show();
                        if (ideraWarningMessageBox.isDeleteClick)
                        {
                            Constants.isDeleteAction = true;
                            InstallProperties.UpgradeSchema = false;
                            //IderaMessageBox messageBox = new IderaMessageBox();
                            //messageBox.ShowYesNo("Are you sure you want to delete (SQLcompliance and SQLcomplianceProcessing) database?", "Idera SQL Compliance Manager");
                            //if (messageBox.isYesClick)
                            //{
                            //    if (!helperFunction.IsSQLComplianceDBRemove(InstallProperties.IsCMSQLServerSQLAuth, InstallProperties.CMSQLServerInstanceName, InstallProperties.CMSQLServerUsername, InstallProperties.CMSQLServerPassword, out errorMessage))
                            //    {
                            //        errorMessage = errorMessage.ToString();
                            //        this.Cursor = Cursors.Default;
                            //        return false;
                            //    }
                            //}
                            //else
                            //{
                            //    Host.NavigateToPage(NavigationDirection.Previous);
                            //}
                        }
                        if (ideraWarningMessageBox.isOkClick)
                        {
                            Constants.isDeleteAction = false;
                            if (!helperFunction.IsSQLComplianceDBCheck(InstallProperties.IsCMSQLServerSQLAuth, InstallProperties.CMSQLServerInstanceName, InstallProperties.CMSQLServerUsername, InstallProperties.CMSQLServerPassword, out errorMessage))
                            {
                                errorMessage = errorMessage.ToString();
                                this.Cursor = Cursors.Default;
                                return false;
                            }
                        }
                        if (ideraWarningMessageBox.isChangeClick)
                        {
                            Constants.isDeleteAction = false;
                            if (InstallProperties.UpgradeSchema == false)
                                InstallProperties.UpgradeSchema = false;
                            Host.NavigateToPage(NavigationDirection.Previous);
                        }
                        if (ideraWarningMessageBox.isExitClick)
                        {
                            IderaMessageBox messageBox = new IderaMessageBox();
                            messageBox.ShowYesNo("Are you sure you want to cancel IDERA SQL Compliance Manager installation?", "Idera SQL Compliance Manager");
                            if (messageBox.isYesClick)
                            {
                                Host.CleanupDirectories();
                                Host.ExitApplication();
                            }
                            else
                            {
                                Host.NavigateToPage(NavigationDirection.Previous);
                            }
                            Constants.isDeleteAction = false;
                            //Host.CleanupDirectories();
                            //Host.ExitApplication();
                        }
                        //errorMessage = "Database(s) named 'SQLcompliance' and/or 'SQLcomplianceProcessing' are found in selected instance. To continue with SQL Compliance Manager installation, delete the database(s) from selected instance and click Next.";
                        //this.Cursor = Cursors.Default;
                        //return false;
                    }
                }
            }

            if (!InstallProperties.IsUpgradeRadioSelection)
            {
                if (!helperFunction.GetSQLServerServiceAccount(InstallProperties.IsCMSQLServerSQLAuth, InstallProperties.CMSQLServerInstanceName, InstallProperties.CMSQLServerUsername, InstallProperties.CMSQLServerPassword, out errorMessage))
                {
                    this.Cursor = Cursors.Default;
                    return false;
                }
            }

            this.Cursor = Cursors.Default;
            return true;
        }

        public void SetControlsForUpgrade()
        {
            if (Constants.UserInstallType == InstallType.CMAndDashboard || Constants.UserInstallType == InstallType.CMOnly)
            {
                panelSQLCMRepository.Visible = true;
                panelDashboardRepositoryHeader.Visible = false;
                panelDashboardRepository.Visible = false;
                panelClusteredEnv.Visible = false;
                panelNode.Visible = false;
                panelSQLCMRepository.Location = new Point(0, 64);
                dropdownCMSQLServerInstance.Enabled = false;
                labelSubHeader.Text = "Specify the SQL Server Credentials required for " + Constants.ProductMap[Products.Compliance] + " Upgrade";
            }
        }

        private bool ValidateCredentials(out string errorMessage)
        {
            HelperFunctions helperFunction = new HelperFunctions();
            if (Constants.UserInstallType == InstallType.CMOnly || Constants.UserInstallType == InstallType.AgentOnly ||
                Constants.UserInstallType == InstallType.CMAndDashboard || Constants.UserInstallType == InstallType.AgentAndDashboard)
            {
                if (!helperFunction.CheckServerConnection(InstallProperties.IsCMSQLServerSQLAuth, InstallProperties.CMSQLServerInstanceName, InstallProperties.CMSQLServerUsername, InstallProperties.CMSQLServerPassword, out errorMessage))
                {
                    errorMessage = "Connection failed for " + Constants.ProductMap[Products.Compliance] + " Repository. Please check Server Instance and Authentication.";
                    return false;
                }
            }

            if ((Constants.UserInstallType == InstallType.DashboardOnly ||
                Constants.UserInstallType == InstallType.CMAndDashboard || Constants.UserInstallType == InstallType.AgentAndDashboard) && !InstallProperties.IsUpgradeRadioSelection)
            {
                if (!helperFunction.CheckServerConnection(InstallProperties.IsDashboardSQLServerSQLAuth, InstallProperties.DashboardSQLServerInstanceName, InstallProperties.DashboardSQLServerUsername, InstallProperties.DashboardSQLServerPassword, out errorMessage))
                {
                    errorMessage = "Connection failed for " + Constants.ProductMap[Products.Dashboard] + " Repository. Please check Server Instance and Authentication.";
                    return false;
                }
            }
            errorMessage = null;
            return true;
        }

        private void linkLabelChangeCMCred_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            FormCMSQLServerCred formCMSQLServerCred = new FormCMSQLServerCred();
            formCMSQLServerCred.ShowDialog();
            if (InstallProperties.CMSQLServerUsername.Equals(string.Empty) && InstallProperties.CMSQLServerPassword.Equals(string.Empty))
            {
                radioCMWindowAuth.Checked = true;
                radioCMSQLServerAuth.Checked = false;
            }
            else
            {
                txtCMSQLServerUser.Text = InstallProperties.CMSQLServerUsername;
                if (checkBoxSameCredAsCM.Checked)
                {
                    InstallProperties.DashboardSQLServerUsername = InstallProperties.CMSQLServerUsername;
                    InstallProperties.DashboardSQLServerPassword = InstallProperties.CMSQLServerPassword;
                    txtDashboardSQLServerUser.Text = txtCMSQLServerUser.Text;
                }
            }
        }

        private void linkLabelChangeDashboardCred_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            FormDashboardSQLServerCred formDashboardSQLServerCred = new FormDashboardSQLServerCred();
            formDashboardSQLServerCred.ShowDialog();
            if (InstallProperties.DashboardSQLServerUsername.Equals(string.Empty) && InstallProperties.DashboardSQLServerPassword.Equals(string.Empty))
            {
                radioDashboardWindowAuth.Checked = true;
                radioDashboardSQLServerAuth.Checked = false;
            }
            else
            {
                txtDashboardSQLServerUser.Text = InstallProperties.DashboardSQLServerUsername;
            }
        }

        private void dropdownCMSQLServerInstance_Leave(object sender, EventArgs e)
        {
            InstallProperties.CMSQLServerInstanceName = dropdownCMSQLServerInstance.SelectedItem;
        }

        private void dropdownDashboardSQLServerInstance_Leave(object sender, EventArgs e)
        {
            InstallProperties.DashboardSQLServerInstanceName = dropdownDashboardSQLServerInstance.SelectedItem;
        }

        private void GetSQLServerInstanceFromCMRegistry()
        {
            localSQLServerInstanceList = new List<string>();
            string instanceToBeUpgraded = "";

            RegistryView registryView = Environment.Is64BitOperatingSystem ? RegistryView.Registry64 : RegistryView.Registry32;
            using (RegistryKey hklm = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, registryView))
            {
                RegistryKey instanceKey;
                if (hklm.OpenSubKey(@"SOFTWARE\Idera\SQLCM\CollectionService", false) != null)
                {
                    instanceKey = hklm.OpenSubKey(@"SOFTWARE\Idera\SQLCM\CollectionService", false);
                }
                else
                {
                    instanceKey = hklm.OpenSubKey(@"SOFTWARE\Idera\SQLcompliance\CollectionService", false);
                }

                if (instanceKey != null)
                {
                    instanceToBeUpgraded = instanceKey.GetValue("ServerInstance").ToString();
                    localSQLServerInstanceList.Add(instanceToBeUpgraded);
                }
            }
        }

        private void dropdownCMSQLServerInstance_TextChanged(object sender, EventArgs e)
        {
            if (checkBoxSameCredAsCM.Checked)
            {
                this.dropdownDashboardSQLServerInstance.SetDefaultItem(this.dropdownCMSQLServerInstance.SelectedItem);
                InstallProperties.DashboardSQLServerInstanceName = this.dropdownCMSQLServerInstance.SelectedItem;
            }
        }
    }
}
