using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CWFInstallerService;

namespace Installer_form_application
{
    public partial class RepositoryDetails : Form
    {
        Form screenObject;
        public RepositoryDetails(Form screenObj)
        {
            InitializeComponent();
            MinimizeBox = false;
            MaximizeBox = false;
            screenObject = screenObj;
        }


        private void buttonNext_Click(object sender, EventArgs e)
        {
            properties.IDDBName = textBoxIDDBName.Text;
            properties.IDInstance = textBoxIDInstance.Text;
            if (checkBoxTagging.Checked)
            {
                properties.isTaggableSampleProduct = true;
            }
            else
            {
                properties.isTaggableSampleProduct = false;
            }
            this.Hide();
            validateRepositoryCreds();         
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            if (DialogResult.Yes == MessageBox.Show("Do you really want to exit?", "Exit", MessageBoxButtons.YesNo))
            {
                Application.Exit();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SQLAuthID newScreen = new SQLAuthID(this);
            newScreen.Show();
        }

        private void checkBoxUseAuthID_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxUseAuthID.Checked)
            {
                buttonChangeID.Enabled = true;
                SQLAuthID newScreen = new SQLAuthID(this);
                newScreen.ShowDialog();
            }
            else
            {
                buttonChangeID.Enabled = false;
            }
        }

        public void disableCheckBox()
        {
            checkBoxUseAuthID.Checked = false;
        }

        public void StoreSQLAuth(string username, string password)
        {
            properties.SQLUsernameID = username;
            properties.SQLPasswordID = password;
            buttonChangeID.Enabled = true;
            this.Show();
        }

        private void validateRepositoryCreds()
        {
            string hostname, username, password;
            bool sqlAuth = false, is2FA = false;
            if (textBoxIDInstance.Text == "(local)")
            {
                hostname = Environment.MachineName;
            }
            else
            {
                hostname = textBoxIDInstance.Text;
            }
            if (checkBoxUseAuthID.Checked)
            {
                sqlAuth = true;
                username = properties.SQLUsernameID;
                password = properties.SQLPasswordID;
            }
            else
            {
                username = properties.IDSUsername;
                password = properties.IDSPassword;
            }
            try
            {
                Validator.ValidateRepositoryConnection(hostname, textBoxIDDBName.Text, sqlAuth, username, password, is2FA, ref properties.dbExists, "Idera Dashboard");
            }
            catch (CWFBaseException ex)
            {
                MessageBox.Show(ex.ErrorCode + " - " + ex.ErrorMessage, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Show();
                return;
            }
            if (checkBoxUseAuthID.Checked)
            {
                properties.SQLAUTHID = true;
            }
            else
            {
                properties.SQLAUTHID = false;
            }
            RepositoryDetailsCM nextScreen = new RepositoryDetailsCM(this);
            nextScreen.Show();
        }

        private void RepositoryDetails_Load(object sender, EventArgs e)
        {
            if (!properties.localInstall)
            {
                textBoxIDInstance.Text = properties.RemoteHostname;
            }
            if (properties.upgrading || !properties.installDashboard)
            {
                textBoxIDDBName.Enabled = false;
                textBoxIDInstance.Enabled = false;
                checkBoxUseAuthID.Enabled = false;
            }
            textBoxIDInstance.Text = properties.IDInstance;
            textBoxIDDBName.Text = properties.IDDBName;
            if (properties.SQLAUTHID) checkBoxUseAuthID.Checked = true;
        }

        private void buttonBack_Click_1(object sender, EventArgs e)
        {
            properties.IDInstance = textBoxIDInstance.Text;
            properties.IDDBName = textBoxIDDBName.Text;
            if(checkBoxUseAuthID.Checked) properties.SQLAUTHID = true;
            else properties.SQLAUTHID = false;
            this.Hide();
            screenObject.Show();
        }
    }
}
