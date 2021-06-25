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
    public partial class CwfAddinInstall : Form
    {
        public CwfAddinInstall()
        {
            InitializeComponent();
        }

        private void CwfAddinInstall_Load(object sender, EventArgs e)
        {
            cwfAddinProgress.Text = "Please wait while CwfAddins are installed on your system.";
            cwfAddinProgress.Show();
        }

        public void RefreshLabel()
        {
            cwfAddinProgress.Refresh();
        }
    }
}
