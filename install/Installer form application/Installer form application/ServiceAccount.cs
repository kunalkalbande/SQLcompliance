using CWFInstallerService;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Forms;

namespace Installer_form_application
{
    public partial class ServiceAccount : Form
    {
        Form backScreenObj;
        public ServiceAccount(Form backScreen)
        {
            InitializeComponent();
            MinimizeBox = false;
            MaximizeBox = false;
            backScreenObj = backScreen;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxSameCreds.Checked)
            {
                labelIDSPassword.Visible = false;
                labelIDSUsername.Visible = false;
                textBoxIDSPassword.Visible = false;
                textBoxIDSUsername.Visible = false;
                labelSACredentials.Visible = false;
                labelSASPCredentials.Visible = false;
            }
            else
            {
                labelIDSPassword.Visible = true;
                labelIDSUsername.Visible = true;
                textBoxIDSPassword.Visible = true;
                textBoxIDSUsername.Visible = true;
                labelSACredentials.Visible = true;
                labelSASPCredentials.Visible = true;
            }
        }

        private void buttonBack_Click(object sender, EventArgs e)
        {
            properties.IDSUsername = textBoxIDSUsername.Text;
            properties.IDSPassword = textBoxIDSPassword.Text;
            properties.SPSPassword = textBoxSPSPassword.Text;
            properties.SPSUsername = textBoxSPSUserName.Text;
            this.Hide();
            backScreenObj.Show();
        }

        private void buttonNext_Click(object sender, EventArgs e)
        {
            this.Hide();
            if (checkBoxSameCreds.Checked)
            {
                textBoxIDSUsername.Text = textBoxSPSUserName.Text;
                textBoxIDSPassword.Text = textBoxSPSPassword.Text;
            }
            try
            {
                if (properties.localInstall)
                {
                    Validator.ValidateServiceCredentials(textBoxIDSUsername.Text, textBoxIDSPassword.Text);
                    Validator.ValidateServiceCredentials(textBoxSPSUserName.Text, textBoxSPSPassword.Text);
                }
                else
                {
                    InstallationHelper.validateRemoteServiceAccount(textBoxIDSUsername.Text, textBoxIDSPassword.Text, properties.RemoteHostname);
                    InstallationHelper.validateRemoteServiceAccount(textBoxSPSUserName.Text, textBoxSPSPassword.Text, properties.RemoteHostname);
                }
            }
            catch (CWFBaseException ex)
            {
                MessageBox.Show(ex.ErrorCode + " - " + ex.ErrorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Show();
                return;
            }

            String MachineName = Dns.GetHostName();
            String[] DashboardUserName = textBoxIDSUsername.Text.Split('\\');
            String[] SPUserName = textBoxSPSUserName.Text.Split('\\');
            if (DashboardUserName[0] == "." || DashboardUserName[0] == "(local)")
            {
                textBoxIDSUsername.Text = String.Empty;
                DashboardUserName[0] = MachineName;
            }
            if (SPUserName[0] == "." || SPUserName[0] == "(local)")
            {
                textBoxSPSUserName.Text = String.Empty;
                SPUserName[0] = MachineName;
            }

            textBoxIDSUsername.Text = DashboardUserName[0] + "\\" + DashboardUserName[1];
            textBoxSPSUserName.Text = SPUserName[0] + "\\" + SPUserName[1];

            properties.IDSUsername = textBoxIDSUsername.Text;
            properties.IDSPassword = textBoxIDSPassword.Text;
            properties.SPSPassword = textBoxSPSPassword.Text;
            properties.SPSUsername = textBoxSPSUserName.Text;
            if (properties.localInstall)
            {
                if (properties.upgrading || !properties.installDashboard)
                {
                    RepositoryDetails nextScreen = new RepositoryDetails(this);
                    nextScreen.Show();
                }
                else
                {
                    PortForm nextScreen = new PortForm(this);
                    nextScreen.Show();
                }
            }
            else
            {
                RepositoryDetailsCM nextScreen = new RepositoryDetailsCM(this);
                nextScreen.Show();
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            if (DialogResult.Yes == MessageBox.Show("Do you really want to exit?", "Exit", MessageBoxButtons.YesNo))
            {
                Application.Exit();
            }
        }

        private void ServiceAccount_Load(object sender, EventArgs e)
        {
            textBoxIDSUsername.Text = properties.IDSUsername;
            textBoxIDSPassword.Text = properties.IDSPassword;
            textBoxSPSUserName.Text = properties.SPSUsername;
            textBoxSPSPassword.Text = properties.SPSPassword;
            if (!properties.localInstall)
            {
                checkBoxSameCreds.Hide();
            }
        }

        
    }
}
