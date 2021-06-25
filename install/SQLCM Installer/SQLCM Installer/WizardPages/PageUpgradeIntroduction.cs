using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SQLCM_Installer.Custom_Controls;
using System.Diagnostics;
using SQLCM_Installer.Custom_Prompts;

namespace SQLCM_Installer.WizardPages
{
    [WizardPageInfo(WizardPage.UpgradeIntroduction)]
    internal partial class PageUpgradeIntroduction : WizardPageBase
    {
        public PageUpgradeIntroduction(MainForm host)
            : base(host)
        {
            InitializeComponent();
        }

        internal override void Initialize()
        {
            IsInitialized = true;
            IsSilentAgentInstalled();
        }

        internal override bool DoAction(out string errorMessage, out Control invalidControl)
        {
            errorMessage = null;
            invalidControl = null;

            if (radioButtonUpgrade.Checked)
            {
                InstallProperties.IsUpgradeRadioSelection = true;
                InstallProperties.IsFreshRadioSelection = false;
                switch (Constants.UserCurrentInstallation)
                {
                    case InstallType.AgentAndDashboard:
                        if (InstallProperties.isDashboardCurrentVersionInstalled)
                        {
                            Constants.UserInstallType = InstallType.AgentOnly;
                        }
                        else if (InstallProperties.isCMCurrentVersionInstalled)
                        {
                            Constants.UserInstallType = InstallType.DashboardOnly;
                        }
                        else
                        {
                            Constants.UserInstallType = Constants.UserCurrentInstallation;
                        }
                        break;
                    case InstallType.ConsoleAndDashboard:
                        if (InstallProperties.isDashboardCurrentVersionInstalled)
                        {
                            Constants.UserInstallType = InstallType.ConsoleOnly;
                        }
                        else if (InstallProperties.isCMCurrentVersionInstalled)
                        {
                            Constants.UserInstallType = InstallType.DashboardOnly;
                        }
                        else
                        {
                            Constants.UserInstallType = Constants.UserCurrentInstallation;
                        }
                        break;
                    case InstallType.CMAndDashboard:
                        if (InstallProperties.isDashboardCurrentVersionInstalled)
                        {
                            Constants.UserInstallType = InstallType.CMOnly;
                        }
                        else if (InstallProperties.isCMCurrentVersionInstalled)
                        {
                            Constants.UserInstallType = InstallType.DashboardOnly;
                        }
                        else
                        {
                            Constants.UserInstallType = Constants.UserCurrentInstallation;
                        }
                        break;
                    default:
                        Constants.UserInstallType = Constants.UserCurrentInstallation;
                        break;
                }
            }
            else if (radioButtonFresh.Checked)
            {
                InstallProperties.IsUpgradeRadioSelection = false;
                InstallProperties.IsFreshRadioSelection = true;
            }
            else
            {
                InstallProperties.IsUpgradeRadioSelection = false;
                InstallProperties.IsFreshRadioSelection = false;
            }

            return true;
        }

        internal override void OnNavigated(NavigationDirection direction)
        {
            this.radioButtonUpgrade.UpdateRadioImageLocation();
            this.radioButtonFresh.UpdateRadioImageLocation();            

            if (Constants.UserCurrentInstallation == InstallType.AgentOnly)
            {
                this.labelUpgradeFirst.Text = "- " + Constants.ProductMap[Products.Agent] + " [" + Constants.InstalledCMVersion + "]";
                this.labelUpgradeFirst.Show();

                this.labelFreshFirst.Text = "- " + Constants.ProductMap[Products.Dashboard];
                this.labelFreshFirst.Show();
            }
            else if (Constants.UserCurrentInstallation == InstallType.ConsoleOnly)
            {
                this.labelUpgradeFirst.Text = "- " + Constants.ProductMap[Products.Console] + " [" + Constants.InstalledCMVersion + "]";
                this.labelUpgradeFirst.Show();

                this.labelFreshFirst.Text = "- " + Constants.ProductMap[Products.Dashboard];
                this.labelFreshFirst.Show();
            }
            else if (Constants.UserCurrentInstallation == InstallType.CMOnly)
            {
                this.labelUpgradeFirst.Text = "- " + Constants.ProductMap[Products.Compliance] + " [" + Constants.InstalledCMVersion + "]";
                this.labelUpgradeFirst.Show();

                if (!InstallProperties.Clustered)
                {
                    this.labelUpgradeSecond.Text = "- " + Constants.ProductMap[Products.Agent] + " [" + Constants.InstalledCMVersion + "]";
                    this.labelUpgradeSecond.Show();

                    this.labelUpgradeThird.Text = "- " + Constants.ProductMap[Products.Console] + " [" + Constants.InstalledCMVersion + "]";
                    this.labelUpgradeThird.Show();
                }
                else
                {
                    this.labelUpgradeSecond.Text = "- " + Constants.ProductMap[Products.Console] + " [" + Constants.InstalledCMVersion + "]";
                    this.labelUpgradeSecond.Show();
                }

                this.labelFreshFirst.Text = "- " + Constants.ProductMap[Products.Dashboard];
                this.labelFreshFirst.Show();
            }
            else if (Constants.UserCurrentInstallation == InstallType.DashboardOnly)
            {
                this.labelUpgradeFirst.Text = "- " + Constants.ProductMap[Products.Dashboard] + " [" + Constants.InstalledDashboardVersion + "]";
                this.labelUpgradeFirst.Show();

                if (InstallProperties.isSilentAgentInstalled)
                {
                    this.labelFreshFirst.Text = "- " + Constants.ProductMap[Products.Console];
                    this.labelFreshFirst.Show();
                }
                else
                {
                    this.labelFreshFirst.Text = "- " + Constants.ProductMap[Products.Compliance];
                    this.labelFreshFirst.Show();

                    this.labelFreshSecond.Text = "- " + Constants.ProductMap[Products.Agent];
                    this.labelFreshSecond.Show();

                    this.labelFreshThird.Text = "- " + Constants.ProductMap[Products.Console];
                    this.labelFreshThird.Show();
                }
            }
            else if (Constants.UserCurrentInstallation == InstallType.AgentAndDashboard)
            {
                this.labelUpgradeFirst.Text = "- " + Constants.ProductMap[Products.Agent] + " [" + Constants.InstalledCMVersion + "]";
                this.labelUpgradeFirst.Show();

                if (InstallProperties.isCMCurrentVersionInstalled)
                {
                    this.labelUpgradeFirst.Text = "- " + Constants.ProductMap[Products.Agent] + " [" + Constants.InstalledCMVersion + "] - Not required";
                    this.labelUpgradeFirst.Show();
                    this.labelUpgradeFirst.ForeColor = Color.FromArgb(146, 136, 117);
                }
                if (InstallProperties.isDashboardCurrentVersionInstalled)
                {
                    this.labelUpgradeSecond.Text = "- " + Constants.ProductMap[Products.Dashboard] + " [" + Constants.InstalledDashboardVersion + "] - Not required";
                    this.labelUpgradeSecond.Show();
                    this.labelUpgradeSecond.ForeColor = Color.FromArgb(146, 136, 117);
                }

                else
                {
                    this.labelUpgradeSecond.Text = "- " + Constants.ProductMap[Products.Dashboard] + " [" + Constants.InstalledDashboardVersion + "]";
                    this.labelUpgradeSecond.Show();
                }

                this.labelFreshFirst.Text = "- " + Constants.ProductMap[Products.NA];
                this.labelFreshFirst.Show();

                this.radioButtonFresh.Disabled = true;
                this.labelFreshText.ForeColor = Color.FromArgb(146, 136, 117);
                this.labelFreshFirst.ForeColor = Color.FromArgb(146, 136, 117);

                this.radioButtonFresh.Location = new Point(24, 200);
                this.labelFreshText.Text = "No new product applicable for fresh installation.";
                this.labelFreshText.Location = new Point(40, 230);
                this.labelFreshFirst.Location = new Point(65, 255);
            }
            else if (Constants.UserCurrentInstallation == InstallType.ConsoleAndDashboard)
            {
                this.labelUpgradeFirst.Text = "- " + Constants.ProductMap[Products.Console] + " [" + Constants.InstalledCMVersion + "]";
                this.labelUpgradeFirst.Show();

                if (InstallProperties.isCMCurrentVersionInstalled)
                {
                    this.labelUpgradeFirst.Text = "- " + Constants.ProductMap[Products.Console] + " [" + Constants.InstalledCMVersion + "] - Not required";
                    this.labelUpgradeFirst.Show();
                    this.labelUpgradeFirst.ForeColor = Color.FromArgb(146, 136, 117);
                }

                if (InstallProperties.isDashboardCurrentVersionInstalled)
                {
                    this.labelUpgradeSecond.Text = "- " + Constants.ProductMap[Products.Dashboard] + " [" + Constants.InstalledDashboardVersion + "] - Not required";
                    this.labelUpgradeSecond.Show();
                    this.labelUpgradeSecond.ForeColor = Color.FromArgb(146, 136, 117);
                }
                else
                {
                    this.labelUpgradeSecond.Text = "- " + Constants.ProductMap[Products.Dashboard] + " [" + Constants.InstalledDashboardVersion + "]";
                    this.labelUpgradeSecond.Show();
                }

                this.labelFreshFirst.Text = "- " + Constants.ProductMap[Products.NA];
                this.labelFreshFirst.Show();

                this.radioButtonFresh.Disabled = true;
                this.labelFreshText.ForeColor = Color.FromArgb(146, 136, 117);
                this.labelFreshFirst.ForeColor = Color.FromArgb(146, 136, 117);

                this.radioButtonFresh.Location = new Point(24, 200);
                this.labelFreshText.Text = "No new product applicable for fresh installation.";
                this.labelFreshText.Location = new Point(40, 230);
                this.labelFreshFirst.Location = new Point(65, 255);
            }
            else if (Constants.UserCurrentInstallation == InstallType.CMAndDashboard)
            {
                this.labelUpgradeFirst.Text = "- " + Constants.ProductMap[Products.Compliance] + " [" + Constants.InstalledCMVersion + "]";
                this.labelUpgradeFirst.Show();

                if (!InstallProperties.Clustered)
                {
                    if (InstallProperties.isCMCurrentVersionInstalled)
                    {
                        this.labelUpgradeFirst.Text = "- " + Constants.ProductMap[Products.Compliance] + " [" + Constants.InstalledCMVersion + "] - Not required";
                        this.labelUpgradeFirst.Show();
                        this.labelUpgradeFirst.ForeColor = Color.FromArgb(146, 136, 117);

                        this.labelUpgradeSecond.Text = "- " + Constants.ProductMap[Products.Agent] + " [" + Constants.InstalledCMVersion + "]";
                        this.labelUpgradeSecond.Show();
                        this.labelUpgradeSecond.ForeColor = Color.FromArgb(146, 136, 117);

                        this.labelUpgradeThird.Text = "- " + Constants.ProductMap[Products.Console] + " [" + Constants.InstalledCMVersion + "]";
                        this.labelUpgradeThird.Show();
                        this.labelUpgradeThird.ForeColor = Color.FromArgb(146, 136, 117);
                    }
                    else
                    {
                        this.labelUpgradeSecond.Text = "- " + Constants.ProductMap[Products.Agent] + " [" + Constants.InstalledCMVersion + "]";
                        this.labelUpgradeSecond.Show();

                        this.labelUpgradeThird.Text = "- " + Constants.ProductMap[Products.Console] + " [" + Constants.InstalledCMVersion + "]";
                        this.labelUpgradeThird.Show();
                    }
                }
                else
                {
                    this.labelUpgradeSecond.Text = "- " + Constants.ProductMap[Products.Console] + " [" + Constants.InstalledCMVersion + "]";
                    this.labelUpgradeSecond.Show();
                }

                if (InstallProperties.isDashboardCurrentVersionInstalled)
                {
                    if (InstallProperties.Clustered)
                    {
                        this.labelUpgradeThird.Text = "- " + Constants.ProductMap[Products.Dashboard] + " [" + Constants.InstalledDashboardVersion + "] - Not required";
                        this.labelUpgradeThird.Show();
                        this.labelUpgradeThird.ForeColor = Color.FromArgb(146, 136, 117);
                    }
                    else
                    {
                        this.labelUpgradeFourth.Text = "- " + Constants.ProductMap[Products.Dashboard] + " [" + Constants.InstalledDashboardVersion + "] - Not required";
                        this.labelUpgradeFourth.Show();
                        this.labelUpgradeFourth.ForeColor = Color.FromArgb(146, 136, 117);
                    }
                }
                else
                {
                    if (InstallProperties.Clustered)
                    {
                        this.labelUpgradeThird.Text = "- " + Constants.ProductMap[Products.Dashboard] + " [" + Constants.InstalledDashboardVersion + "]";
                        this.labelUpgradeThird.Show();
                    }
                    else
                    {
                        this.labelUpgradeFourth.Text = "- " + Constants.ProductMap[Products.Dashboard] + " [" + Constants.InstalledDashboardVersion + "]";
                        this.labelUpgradeFourth.Show();
                    }
                }

                this.labelFreshFirst.Text = "- " + Constants.ProductMap[Products.NA];
                this.labelFreshFirst.Show();

                this.radioButtonFresh.Disabled = true;
                this.labelFreshText.ForeColor = Color.FromArgb(146, 136, 117);
                this.labelFreshFirst.ForeColor = Color.FromArgb(146, 136, 117);

                this.radioButtonFresh.Location = new Point(24, 200);
                this.labelFreshText.Text = "No new product applicable for fresh installation.";
                this.labelFreshText.Location = new Point(40, 230);
                this.labelFreshFirst.Location = new Point(65, 255);
            }
            else if (Constants.UserCurrentInstallation == InstallType.NotSpecified)
            {
                this.labelUpgradeFirst.Text = "- " + Constants.ProductMap[Products.Agent];
                this.labelUpgradeFirst.Show();

                if (InstallProperties.isSilentAgentInstalled)
                {
                    this.labelFreshFirst.Text = "- " + Constants.ProductMap[Products.Console];
                    this.labelFreshFirst.Show();

                    this.labelFreshSecond.Text = "- " + Constants.ProductMap[Products.Dashboard];
                    this.labelFreshSecond.Show();
                }
            }
            else
            {
                // Should not come here ever.
            }

            if (InstallProperties.isCurrentVersionInstalled || InstallProperties.BlockInstallation)
            {
                if (!this.radioButtonFresh.Disabled)
                {
                    this.radioButtonFresh.Checked = true;
                    InstallProperties.IsUpgradeRadioSelection = false;
                    InstallProperties.IsFreshRadioSelection = true;
                }

                this.radioButtonUpgrade.Disabled = true;
                if (InstallProperties.BlockInstallation)
                {
                    this.labelUpgradeText.Text = "Due to conflicting Agent installation, below products can not be upgraded:";
                }
                else
                {
                    this.labelUpgradeText.Text = "The current or greater version is already installed on this system for:";
                }
                this.labelUpgradeFirst.Location = new Point(65, 70);
                this.labelUpgradeSecond.Location = new Point(65, 95);
                this.labelUpgradeThird.Location = new Point(65, 120);
                this.labelUpgradeFourth.Location = new Point(65, 145);
                this.labelUpgradeText.ForeColor = Color.FromArgb(146, 136, 117);
                this.labelUpgradeFirst.ForeColor = Color.FromArgb(146, 136, 117);
                this.labelUpgradeSecond.ForeColor = Color.FromArgb(146, 136, 117);
                this.labelUpgradeThird.ForeColor = Color.FromArgb(146, 136, 117);
                this.labelUpgradeFourth.ForeColor = Color.FromArgb(146, 136, 117);
            }
            else if (InstallProperties.isDashboardCurrentVersionInstalled && Constants.UserCurrentInstallation == InstallType.DashboardOnly)
            {
                if (!this.radioButtonFresh.Disabled)
                {
                    this.radioButtonFresh.Checked = true;
                    InstallProperties.IsUpgradeRadioSelection = false;
                    InstallProperties.IsFreshRadioSelection = true;
                }

                this.radioButtonUpgrade.Disabled = true;
                this.labelUpgradeText.Text = "The current or greater version is already installed on this system for:";

                this.labelUpgradeFirst.Location = new Point(65, 70);
                this.labelUpgradeSecond.Location = new Point(65, 95);
                this.labelUpgradeThird.Location = new Point(65, 120);
                this.labelUpgradeFourth.Location = new Point(65, 145);
                this.labelUpgradeText.ForeColor = Color.FromArgb(146, 136, 117);
                this.labelUpgradeFirst.ForeColor = Color.FromArgb(146, 136, 117);
                this.labelUpgradeSecond.ForeColor = Color.FromArgb(146, 136, 117);
                this.labelUpgradeThird.ForeColor = Color.FromArgb(146, 136, 117);
                this.labelUpgradeFourth.ForeColor = Color.FromArgb(146, 136, 117);
            }
            else if (InstallProperties.isCMCurrentVersionInstalled 
                && (Constants.UserCurrentInstallation == InstallType.AgentOnly
                || Constants.UserCurrentInstallation == InstallType.ConsoleOnly
                || Constants.UserCurrentInstallation == InstallType.CMOnly))
            {
                if (!this.radioButtonFresh.Disabled)
                {
                    this.radioButtonFresh.Checked = true;
                    InstallProperties.IsUpgradeRadioSelection = false;
                    InstallProperties.IsFreshRadioSelection = true;
                }

                this.radioButtonUpgrade.Disabled = true;
                this.labelUpgradeText.Text = "The current or greater version is already installed on this system for:";

                this.labelUpgradeFirst.Location = new Point(65, 70);
                this.labelUpgradeSecond.Location = new Point(65, 95);
                this.labelUpgradeThird.Location = new Point(65, 120);
                this.labelUpgradeFourth.Location = new Point(65, 145);
                this.labelUpgradeText.ForeColor = Color.FromArgb(146, 136, 117);
                this.labelUpgradeFirst.ForeColor = Color.FromArgb(146, 136, 117);
                this.labelUpgradeSecond.ForeColor = Color.FromArgb(146, 136, 117);
                this.labelUpgradeThird.ForeColor = Color.FromArgb(146, 136, 117);
                this.labelUpgradeFourth.ForeColor = Color.FromArgb(146, 136, 117);
            }
        }

        private void linkLabelInstallationRequirements_Click(object sender, EventArgs e)
        {
            Process.Start(@"http://wiki.idera.com/display/SQLCM/Product+requirements");
        }

        private void EULAHyperlink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            FormEulaBox eulaBox = new FormEulaBox();
            eulaBox.ShowDialog();
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

        private void IsSilentAgentInstalled()
        {
            if (InstallProperties.isSilentAgentInstalled)
            {
                if (Environment.Is64BitOperatingSystem)
                {
                    AgentSilentMessageBox messageBox = new AgentSilentMessageBox();
                    messageBox.Show(Constants.SilentAgentInstalled);
                }
                else
                {
                    AgentSilentMessageBox messageBox = new AgentSilentMessageBox();
                    messageBox.Show(Constants.SilentAgentInstalledx86);
                }
            }
        }
    }
}
