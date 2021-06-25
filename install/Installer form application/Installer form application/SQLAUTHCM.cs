using CWFInstallerService;
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
    public partial class SQLAUTHCM : Form
    {
        RepositoryDetailsCM backScreen;
        public SQLAUTHCM(RepositoryDetailsCM backScreenObj)
        {
            InitializeComponent();
            backScreen = backScreenObj;
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            try
            {
                Validator.validateSqlAuthCredentials(textBoxUsername.Text, textBoxPassword.Text);
            }
            catch (CWFBaseException ex)
            {
                MessageBox.Show(ex.ErrorCode + " - " + ex.ErrorMessage, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Show();
                return;
            }
            this.Hide();
            backScreen.StoreSQLAuth(textBoxUsername.Text, textBoxPassword.Text);
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Hide();
            backScreen.disableCheckBox();
            backScreen.Show();
        }

        private void SQLAUTH2_Load(object sender, EventArgs e)
        {
            textBoxUsername.Text = properties.SQLUsername2;
            textBoxPassword.Text = properties.SQLPassword2;   
        }
    }
}
