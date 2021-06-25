using System;
using System.Windows.Forms;

namespace Idera.SQLcompliance.Application.GUI.Forms
{
    public partial class Form_Welcome : Form
    {
        public Form_Welcome()
        {
            InitializeComponent();
        }

        private void button_Yes_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Yes;
        }

        private void button_No_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.No;
        }

        private void helpAudit_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            HelpAlias.ShowHelp(this, HelpAlias.SSHELP_HowAuditingWorks);
        }

        private void helpOptimize_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            HelpAlias.ShowHelp(this, HelpAlias.SSHELP_AuditingBestPractices);
        }
    }
}
