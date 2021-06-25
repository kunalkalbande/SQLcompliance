using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace SQLCM_Installer.WizardPages
{
    [WizardPageInfo( WizardPage.Introduction)]
    internal partial class PageIntroduction : WizardPageBase
    {
        public PageIntroduction(MainForm host)
            : base(host)
        {
            InitializeComponent();
        }

        private void linkLabelInstallationRequirements_Click(object sender, EventArgs e)
        {
            Process.Start(@"http://wiki.idera.com/display/SQLCM/Product+requirements");
        }
    }
}
