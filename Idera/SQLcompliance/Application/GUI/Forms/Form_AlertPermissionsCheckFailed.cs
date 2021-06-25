using System.Diagnostics;
using System.Windows.Forms;

namespace Idera.SQLcompliance.Application.GUI.Forms
{
    public partial class Form_AlertPermissionsCheckFailed : Form
    {
        private const string RtfStart = @"{\rtf1\ansi\ansicpg1252\deff0\deflang1033{\fonttbl{\f0\fnil\fcharset0 Microsoft Sans Serif;}}{\colortbl ;\red0\green0\blue0;\red0\green0\blue255;\red255\green0\blue0;\red0\green128\blue0;}\viewkind4\uc1\pard\f0\fs17";
        private const string RtfEnd = @"}";

        private readonly bool _isRecheckAllowed;

        public Form_AlertPermissionsCheckFailed(int failedChecks, string resolutionSteps, bool isRecheckAllowed)
        {
            InitializeComponent();

            _isRecheckAllowed = isRecheckAllowed;
            if (!isRecheckAllowed)
            {
                btnIgnore.Visible = false;
                btnStay.Text = @"OK";
            }

            lblMessage.Text = string.Format(lblMessage.Text, failedChecks, failedChecks > 1 ? "s": string.Empty);
            txtResolutionSteps.Rtf = string.Format("{0} {1} {2}", RtfStart, resolutionSteps, RtfEnd);
        }

        private void btnStay_Click(object sender, System.EventArgs e)
        {
            DialogResult = _isRecheckAllowed ? DialogResult.OK : DialogResult.Cancel;
            Close();
        }

        private void btnIgnore_Click(object sender, System.EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void txtResolutionSteps_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            Process.Start(e.LinkText);
        }

        private void lnkHelp_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                HelpAlias.ShowHelp(this, HelpAlias.SSHELP_PermissionsCheckFailed);
                lnkHelp.LinkVisited = true;
            }
        }

        private void Form_AlertPermissionsCheckFailed_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            HelpAlias.ShowHelp(this, HelpAlias.SSHELP_PermissionsCheckFailed);
        }
    }
}