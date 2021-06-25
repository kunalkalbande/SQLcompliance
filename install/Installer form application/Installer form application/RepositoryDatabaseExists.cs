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
    public partial class RepositoryDatabaseExists : Form
    {
        Form backScreen;
        public RepositoryDatabaseExists(Form backScreenObj)
        {
            backScreen = backScreenObj;
            InitializeComponent();
        }

        private void buttonBack_Click(object sender, EventArgs e)
        {
            this.Hide();
            backScreen.Show();
        }

        private void buttonNext_Click(object sender, EventArgs e)
        {
            this.Hide();
            InstallReady nextScreen = new InstallReady(this);
            nextScreen.Show();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            if (DialogResult.Yes == MessageBox.Show("Do you really want to exit?", "Exit", MessageBoxButtons.YesNo))
            {
                Application.Exit();
            }
        }

        private void RepositoryDatabaseExists_Load(object sender, EventArgs e)
        {
            labelInstuction.Text = "The specified Idera Dashboard repository is found on " + properties.IDInstance.ToUpper() + ", and it will be updated if necessary to latest version.\n\nClick Next to continue, or Back to change the database.";
        }
    }
}
