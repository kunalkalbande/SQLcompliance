using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Installer_form_application
{
    public partial class RemoteCredentials : Form
    {
        Credentials backScreenObj;
        public RemoteCredentials(Credentials backScreen)
        {
            InitializeComponent();
            backScreenObj = backScreen;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            try
            {
                CWFInstallerService.InstallationHelper.validateRemoteServiceAccount(textBoxUsername.Text, textBoxPassword.Text, textBoxHostname.Text);
                this.Hide();
                backScreenObj.getRemoteCredentials(textBoxUsername.Text, textBoxPassword.Text, textBoxHostname.Text);
            }
            catch(CWFInstallerService.CWFBaseException ex)
            {
                MessageBox.Show(ex.ErrorCode + " - " + ex.ErrorMessage, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Show();
                return;
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Hide();
            backScreenObj.getRemoteCredentials(null, null, null);
        }

        private void RemoteCredentials_Load(object sender, EventArgs e)
        {
            
            textBoxUsername.Text = properties.RemoteUsername;
            textBoxPassword.Text = properties.RemotePassword;
            textBoxHostname.Text = properties.RemoteHostname;
        }
    }
}
