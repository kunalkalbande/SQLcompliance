using CWFInstallerService;
using Microsoft.Win32;
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
    public partial class RepositoryDetailsCM : Form
    {
        Form backScreenObject;
        public RepositoryDetailsCM(Form backScreenObj)
        {
            InitializeComponent();
            MinimizeBox = false;
            MaximizeBox = false;
            backScreenObject = backScreenObj;
        }

        private void buttonNext_Click(object sender, EventArgs e)
        {
            properties.JMInstance = textBoxCMInstance.Text;
            if (properties.JMInstance.StartsWith(".") || properties.JMInstance.ToLower().StartsWith("(local)"))
            {
                if (properties.JMInstance.ToLower().IndexOf("(local)") != -1)
                {
                    string tempInstance = properties.JMInstance;
                    tempInstance = tempInstance.Substring(tempInstance.ToLower().IndexOf("(local)") + 7);
                    properties.JMInstance = "(local)" + tempInstance;
                }
                properties.JMInstance = properties.JMInstance.Replace(".", Dns.GetHostName()).Replace("(local)", Dns.GetHostName());
            }
            properties.CMDBName = textBoxCMDBName.Text;
            this.Hide();
            validateRepositoryCreds();
        }

        private void buttonBack_Click(object sender, EventArgs e)
        {
            properties.JMInstance = textBoxCMInstance.Text;
            properties.CMDBName = textBoxCMDBName.Text;
            if (checkBoxUseAuth.Checked) properties.SQLAUTH2 = true;
            else properties.SQLAUTH2 = false;
            this.Hide();
            backScreenObject.Show();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            if (DialogResult.Yes == MessageBox.Show("Do you really want to exit?", "Exit", MessageBoxButtons.YesNo))
            {
                Application.Exit();
            }
        }

        public void disableCheckBox()
        {
            checkBoxUseAuth.Checked = false;
        }

        public void StoreSQLAuth(string username, string password)
        {
            properties.SQLUsername2 = username;
            properties.SQLPassword2 = password;
            buttonChange.Enabled = true;
            this.Show();
        }

        private void validateRepositoryCreds()
        {
            string hostnameCM, username, password;
            bool sqlAuth = false, is2FA = false;
            if (textBoxCMInstance.Text == "(local)")
            {
                hostnameCM = Environment.MachineName;
            }
            else
            {
                hostnameCM = textBoxCMInstance.Text;
            }
            if (checkBoxUseAuth.Checked)
            {
                sqlAuth = true;
                username = properties.SQLUsername2;
                password = properties.SQLPassword2;
            }
            else
            {
                username = properties.IDSUsername;
                password = properties.IDSPassword;
            }
            try
            {
                bool dbExists = false;
                Validator.ValidateRepositoryConnection(hostnameCM, textBoxCMDBName.Text, sqlAuth, username, password, is2FA, ref dbExists, "SQL Compliance Manager");
            }
            catch (CWFBaseException ex)
            {
                MessageBox.Show(ex.ErrorCode + " - " + ex.ErrorMessage, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Show();
                return;
            }
            if (checkBoxUseAuth.Checked)
            {
                properties.SQLAUTH2 = true;
            }
            else
            {
                properties.SQLAUTH2 = false;
            }
            if (properties.dbExists)
            {
                RepositoryDatabaseExists nextScreen = new RepositoryDatabaseExists(this);
                nextScreen.Show();
            }
            else
            {
                InstallReady nextScreen = new InstallReady(this);
                nextScreen.Show();
            }
        }

        private void buttonChange_Click(object sender, EventArgs e)
        {
            SQLAUTHCM newScreen = new SQLAUTHCM(this);
            newScreen.ShowDialog();
        }

        private void checkBoxUseAuth_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxUseAuth.Checked)
            {
                buttonChange.Enabled = true;
                SQLAUTHCM newScreen = new SQLAUTHCM(this);
                newScreen.ShowDialog();
            }
            else
            {
                buttonChange.Enabled = false;
            }
        }

        private void RepositoryDetailsDM_Load(object sender, EventArgs e)
        {
            if (properties.isSQLCMUpgrade)
            {
                textBoxCMInstance.Text = properties.JMInstance;
                textBoxCMDBName.Enabled = false;
                textBoxCMInstance.Enabled = false;
                checkBoxUseAuth.Enabled = false;
                buttonChange.Enabled = false;
            }
            else
            {
                textBoxCMInstance.Text = properties.JMInstance;
                textBoxCMDBName.Text = properties.CMDBName;
                if (properties.SQLAUTH2) checkBoxUseAuth.Checked = true;
            }
        }
    }
}
