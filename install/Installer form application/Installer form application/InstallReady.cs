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
    public partial class InstallReady : Form
    {
        Form backScreen;
        public InstallReady(Form backScreenObj)
        {
            InitializeComponent();
            backScreen = backScreenObj;
        }

        private void InstallReady_Load(object sender, EventArgs e)
        {
            labelIDSAccount.Text += properties.IDSUsername;
            labeSPSAccount.Text += properties.SPSUsername;
            if (!properties.localInstall)
            {
                labelDesc.Text = "Ready to install SQL Compliance Manager.";
            }
        }

        private void buttonNext_Click(object sender, EventArgs e)
        {
            this.Hide();
            RemoteProgress nextScreen = new RemoteProgress(this);
            nextScreen.Show();
        }

        private void buttonBack_Click(object sender, EventArgs e)
        {
            this.Hide();
            backScreen.Show();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            if (DialogResult.Yes == MessageBox.Show("Do you really want to exit?", "Exit", MessageBoxButtons.YesNo))
            {
                Application.Exit();
            }
        }
    }
}
