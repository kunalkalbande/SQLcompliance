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
using SQLCM_Installer.Properties;
using SQLCM_Installer.Custom_Controls;

namespace SQLCM_Installer.WizardPages
{
    [WizardPageInfo(WizardPage.SetupType)]
    internal partial class PageSetupType : WizardPageBase
    {
        private bool pageInitialized = false;
        public PageSetupType(MainForm host)
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
            if (InstallProperties.IsFreshRadioSelection && !pageInitialized)
            {
                pageInitialized = true;

                //Hide EULA in component fresh installation due that it was added in upgrade view
                this.EULAHyperlink.Hide();
                this.checkBoxLicenseAgreement.Hide();

                switch (Constants.UserCurrentInstallation)
                {
                    case InstallType.AgentOnly:
                        checkBoxSQLCM.Checked = false;
                        checkBoxSQLCM.Disabled = true;

                        checkBoxAgent.Checked = false;
                        checkBoxAgent.Disabled = true;

                        checkBoxManagementConsole.Disabled = true;

                        checkBoxDashboard.Checked = true;
                        break;
                    case InstallType.ConsoleOnly:
                        checkBoxSQLCM.Checked = false;
                        checkBoxSQLCM.Disabled = true;

                        checkBoxAgent.Checked = false;
                        checkBoxAgent.Disabled = true;

                        checkBoxManagementConsole.Disabled = true;

                        checkBoxDashboard.Checked = true;
                        break;
                    case InstallType.CMOnly:
                        checkBoxSQLCM.Checked = false;
                        checkBoxSQLCM.Disabled = true;

                        checkBoxAgent.Checked = false;
                        checkBoxAgent.Disabled = true;

                        checkBoxManagementConsole.Disabled = true;

                        checkBoxDashboard.Checked = true;
                        break;
                    case InstallType.DashboardOnly:
                        if (InstallProperties.isSilentAgentInstalled)
                        {
                            checkBoxSQLCM.Checked = false;
                            checkBoxSQLCM.Disabled = true;
                            checkBoxAgent.Disabled = true;
                            checkBoxManagementConsole.Checked = true;

                            checkBoxDashboard.Checked = false;
                            checkBoxDashboard.Disabled = true;
                        }
                        else
                        {
                            checkBoxSQLCM.Checked = true;

                            checkBoxDashboard.Checked = false;
                            checkBoxDashboard.Disabled = true;
                        }
                        break;
                    case InstallType.NotSpecified:
                        if (InstallProperties.isSilentAgentInstalled)
                        {
                            checkBoxSQLCM.Checked = false;
                            checkBoxSQLCM.Disabled = true;
                            checkBoxAgent.Disabled = true;
                            checkBoxManagementConsole.Checked = true;
                            checkBoxDashboard.Checked = true;
                            checkBoxDashboard.Disabled = false;
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        private void sqlcmInfoPicture_MouseEnter(object sender, EventArgs e)
        {
            toolTipSQLCM.Visible = true;
        }

        private void sqlcmInfoPicture_MouseLeave(object sender, EventArgs e)
        {
            toolTipSQLCM.Visible = false;
        }

        private void sqlcmManagementConsoleInfoPicture_MouseEnter(object sender, EventArgs e)
        {
            toolTipSQLCMManagamanetConsole.Visible = true;
        }

        private void sqlcmManagementConsoleInfoPicture_MouseLeave(object sender, EventArgs e)
        {
            toolTipSQLCMManagamanetConsole.Visible = false;
        }

        private void sqlcmAgentInfoPicture_MouseEnter(object sender, EventArgs e)
        {
            toolTipAgent.Visible = true;
        }

        private void sqlcmAgentInfoPicture_MouseLeave(object sender, EventArgs e)
        {
            toolTipAgent.Visible = false;
        }

        private void dashboardInfoPicture_MouseEnter(object sender, EventArgs e)
        {
            toolTipDashboard.Visible = true;
        }

        private void dashboardInfoPicture_MouseLeave(object sender, EventArgs e)
        {
            toolTipDashboard.Visible = false;
        }

        private void EULAHyperlink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            FormEulaBox eulaBox = new FormEulaBox();
            eulaBox.ShowDialog();
        }

        private void checkBoxSQLCM_Click(object sender, EventArgs e)
        {
            if (checkBoxSQLCM.Checked)
            {
                checkBoxManagementConsole.Checked = true;
                checkBoxAgent.Checked = true;
                checkBoxManagementConsole.Disabled = true;
                checkBoxAgent.Disabled = true;
            }
            else
            {
                checkBoxManagementConsole.Disabled = false;
                checkBoxAgent.Disabled = false;
                checkBoxManagementConsole.Checked = false;
                checkBoxAgent.Checked = false;
                pictureSetupType.Image = Resources.diagramDashboardOnly;
            }
            SetSetupTypeDiagram();
        }

        private void checkBoxManagementConsole_Click(object sender, EventArgs e)
        {
            if (!checkBoxSQLCM.Checked && checkBoxManagementConsole.Checked)
            {
                if (!InstallProperties.isSilentAgentInstalled)
                {
                    checkBoxAgent.Checked = false;
                }
                pictureSetupType.Image = Resources.diagramCMCOnsoleOnly;
            }
            SetSetupTypeDiagram();
        }

        private void checkBoxAgent_Click(object sender, EventArgs e)
        {
            if (!checkBoxSQLCM.Checked && checkBoxAgent.Checked)
            {
                checkBoxManagementConsole.Checked = false;
                pictureSetupType.Image = Resources.diagramCMAgentOnly;
            }
            SetSetupTypeDiagram();
        }

        private void checkBoxDashboard_Click(object sender, EventArgs e)
        {
            SetSetupTypeDiagram();
        }

        private void SetSetupTypeDiagram()
        {
            if (checkBoxSQLCM.Checked && checkBoxDashboard.Checked)
            {
                pictureSetupType.Image = Resources.diagramAllComponents;
            }
            else if (checkBoxSQLCM.Checked && !checkBoxDashboard.Checked)
            {
                pictureSetupType.Image = Resources.diagramCMAll;
            }
            else if (checkBoxManagementConsole.Checked && !checkBoxDashboard.Checked)
            {
                pictureSetupType.Image = Resources.diagramCMCOnsoleOnly;
            }
            else if (checkBoxManagementConsole.Checked && checkBoxDashboard.Checked)
            {
                pictureSetupType.Image = Resources.diagramConsoleAndDashboard;
            }
            else if (checkBoxAgent.Checked && !checkBoxDashboard.Checked)
            {
                pictureSetupType.Image = Resources.diagramCMAgentOnly;
            }
            else if (checkBoxAgent.Checked && checkBoxDashboard.Checked)
            {
                pictureSetupType.Image = Resources.diagramAgentAndDashboard;
            }
            else if (checkBoxDashboard.Checked)
            {
                pictureSetupType.Image = Resources.diagramDashboardOnly;
            }
            else
            {
                pictureSetupType.Image = Resources.diagramnone;
            }
        }

        private void checkBoxLicenseAgreement_Click(object sender, EventArgs e)
        {
            if (checkBoxLicenseAgreement.Checked)
            {
                InstallProperties.AgreeToLicense = true;
                Host.EnableNext(true);
            }
            else
            {
                InstallProperties.AgreeToLicense = false;
                Host.DisableNext(true);
            }
        }

        internal override bool DoAction(out string errorMessage, out Control invalidControl)
        {
            bool isDashboardInstall = false;
            errorMessage = null;
            invalidControl = null;
            if (checkBoxDashboard.Checked)
            {
                isDashboardInstall = true;
                Constants.UserInstallType = InstallType.DashboardOnly;
            }
            else if (!checkBoxDashboard.Checked)
            {
                isDashboardInstall = false;
            }

            if (checkBoxSQLCM.Checked)
            {
                if (isDashboardInstall)
                {
                    Constants.UserInstallType = InstallType.CMAndDashboard;
                }
                else
                {
                    Constants.UserInstallType = InstallType.CMOnly;
                }
            }
            else if (!checkBoxSQLCM.Checked && checkBoxManagementConsole.Checked)
            {
                if (isDashboardInstall)
                {
                    Constants.UserInstallType = InstallType.ConsoleAndDashboard;
                }
                else
                {
                    Constants.UserInstallType = InstallType.ConsoleOnly;
                }
            }
            else if (!checkBoxSQLCM.Checked && checkBoxAgent.Checked)
            {
                if (isDashboardInstall)
                {
                    Constants.UserInstallType = InstallType.AgentAndDashboard;
                }
                else
                {
                    Constants.UserInstallType = InstallType.AgentOnly;
                }
            }
            else
            {
                if (!isDashboardInstall)
                {
                    errorMessage = "Please select a setup type to proceed";
                    return false;
                }
            }

            return true;
        }
    }
}
