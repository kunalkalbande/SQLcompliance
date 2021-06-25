using System.Management;
using System.Windows.Forms;
using Idera.SQLcompliance.Core;

namespace Idera.SQLcompliance.Application.GUI.Forms
{
    public partial class Form_AlertPermissionsCheck : Form
    {
        public Form_AlertPermissionsCheck()
        {
            InitializeComponent();

            lblAgentServicePermissionsCheck.Text = string.Format(lblAgentServicePermissionsCheck.Text, CoreHelpers.GetServiceAccount(CoreConstants.AgentServiceName));
            lblCollectionServicePermissionsCheck.Text = string.Format(lblCollectionServicePermissionsCheck.Text, CoreHelpers.GetServiceAccount(CoreConstants.CollectionServiceName));
        }

        private void btnYes_Click(object sender, System.EventArgs e)
        {
            DialogResult = DialogResult.Yes;
            Close();
        }

        private void btnNo_Click(object sender, System.EventArgs e)
        {
            DialogResult = DialogResult.No;
            Close();
        }
    }
}