using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CWFInstallerService;
using System.IO;
using System.Security.Principal;
using System.Security.AccessControl;

namespace SQLCM_Installer.WizardPages
{
    [WizardPageInfo(WizardPage.ServiceAccount)]
    internal partial class PageServiceAccount : WizardPageBase
    {
        string ServiceCredErrorMessage;
        public PageServiceAccount(MainForm host)
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
                SetPageControlsAsSetupType();
        }

        public void SetPageControlsAsSetupType()
        {
            if (InstallProperties.IsUpgradeRadioSelection)
            {
                if (Constants.UserCurrentInstallation == InstallType.CMOnly)
                {
                    labelTitle.Text = Constants.InstallCMOnlyTitleServiceAccountScreen;
                }
                else if (Constants.UserCurrentInstallation == InstallType.CMAndDashboard && InstallProperties.IsMajorUpgrade)
                {
                    labelTitle.Text = Constants.InstallCMandDashboardTitleServiceAccountScreen;
                }
                else if (Constants.UserInstallType == InstallType.AgentOnly && InstallProperties.IsMajorUpgrade)
                {
                    labelTitle.Text = Constants.InstallAgentOnlyTitleServiceAccountScreen;
                }
                else if (Constants.UserCurrentInstallation == InstallType.AgentAndDashboard && InstallProperties.IsMajorUpgrade)
                {
                    labelTitle.Text = Constants.InstallAgentandDashboardTitleServiceAccountScreen;
                }
                else
                {
                    labelTitle.Text = Constants.InstallDashboardOnlyTitleServiceAccountScreen;
                }
            }
            else if (Constants.UserInstallType == InstallType.CMOnly)
            {
                labelTitle.Text = Constants.InstallCMOnlyTitleServiceAccountScreen;
            }
            else if (Constants.UserInstallType == InstallType.DashboardOnly)
            {
                labelTitle.Text = Constants.InstallDashboardOnlyTitleServiceAccountScreen;
            }
            else if (Constants.UserInstallType == InstallType.AgentOnly)
            {
                labelTitle.Text = Constants.InstallAgentOnlyTitleServiceAccountScreen;
            }
            else if (Constants.UserInstallType == InstallType.CMAndDashboard)
            {
                labelTitle.Text = Constants.InstallCMandDashboardTitleServiceAccountScreen;
            }
            else if (Constants.UserInstallType == InstallType.ConsoleAndDashboard)
            {
                labelTitle.Text = Constants.InstallDashboardOnlyTitleServiceAccountScreen;
            }
            else if (Constants.UserInstallType == InstallType.AgentAndDashboard)
            {
                labelTitle.Text = Constants.InstallAgentandDashboardTitleServiceAccountScreen;
            }
        }

        internal override bool DoAction(out string errorMessage, out Control invalidControl)
        {
            errorMessage = null;
            invalidControl = null;
            string credentialError = "Authentication failed for {0}";

            if (txtServiceUserName.Text == "" || txtServicePassword.Text == "")
            {
                pictureCredentialError.Visible = true;
                labelCredentialError.Text = "Authentication Failed";
                labelCredentialError.Visible = true;
                return false;
            }

            if (!ValidateServiceCredentials())
            {
                pictureCredentialError.Visible = true;
                labelCredentialError.Text = string.Format(credentialError, txtServiceUserName.Text);
                labelCredentialError.Visible = true;
                return false;
            }
            else
            {
                InstallProperties.ServiceUserName = txtServiceUserName.Text;
                InstallProperties.ServicePassword = txtServicePassword.Text;
                if (!SetFolderAccess(out errorMessage))
                {
                    return false;
                }
                if (Host.CwfHelper.CwfUser == "" && Host.CwfHelper.CwfPassword == "")
                {
                    Host.CwfHelper.CwfUser = InstallProperties.ServiceUserName;
                    Host.CwfHelper.CwfPassword = InstallProperties.ServicePassword;
                    Host.CwfHelper.CwfUrl = string.Format("http://{0}:{1}", Environment.MachineName, "9292");
                }
                
                pictureCredentialError.Visible = false;
                labelCredentialError.Visible = false;
                return true;
            }
        }

        private bool ValidateServiceCredentials()
        {
            try
            {
                Validator.ValidateServiceCredentials(txtServiceUserName.Text, txtServicePassword.Text);
                return true;
            }
            catch (CWFBaseException ex)
            {
                ServiceCredErrorMessage = ex.ErrorMessage;
                return false;
            }
        }

        private bool SetFolderAccess(out string errorMessage)
        {
            errorMessage = "";
            HelperFunctions helperFunctions = new HelperFunctions();
            try
            {
                if (!InstallProperties.IsUpgradeRadioSelection)
                {
                    if (Constants.UserInstallType == InstallType.CMOnly || Constants.UserInstallType == InstallType.CMAndDashboard)
                    {
                        if (!InstallProperties.Clustered)
                        {
                            helperFunctions.SetFolderAccess(true, InstallProperties.CollectionTraceDirectory, Constants.CollectionTrace, out errorMessage);
                            helperFunctions.SetFolderAccess(true, InstallProperties.CMInstallDir, Constants.InstallationDir, out errorMessage);
                            return true;
                        }
                        else
                        {
                            if (InstallProperties.IsActiveNode)
                            {
                                helperFunctions.SetFolderAccess(true, InstallProperties.CollectionTraceDirectory, Constants.CollectionTrace, out errorMessage);
                            }
                            helperFunctions.SetFolderAccess(true, InstallProperties.CMInstallDir, Constants.InstallationDir, out errorMessage);
                            return true;
                        }
                    }
                    else if (Constants.UserInstallType == InstallType.AgentOnly || Constants.UserInstallType == InstallType.AgentAndDashboard)
                    {
                        helperFunctions.SetFolderAccess(true, InstallProperties.AgentTraceDirectory, Constants.AgentTrace, out errorMessage);
                        helperFunctions.SetFolderAccess(true, InstallProperties.CMInstallDir, Constants.InstallationDir, out errorMessage);
                        return true;
                    }
                    else if (Constants.UserInstallType == InstallType.ConsoleOnly || Constants.UserInstallType == InstallType.ConsoleAndDashboard)
                    {
                        helperFunctions.SetFolderAccess(true, InstallProperties.CMInstallDir, Constants.InstallationDir, out errorMessage);
                        return true;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

    }
}
