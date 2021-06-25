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
    public partial class InstallationFailure : Form
    {
        public InstallationFailure()
        {
            InitializeComponent();
        }

        private void buttonFinish_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void InstallationFailure_Load(object sender, EventArgs e)
        {
            labelHeadDescription.Text = Constants.installationFailureErrorMessage;
        }
    }
}
