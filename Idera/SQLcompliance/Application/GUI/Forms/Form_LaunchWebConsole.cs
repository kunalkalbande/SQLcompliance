using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using Idera.SQLcompliance.Application.GUI.Properties;
using Idera.SQLcompliance.Core.Cwf;

namespace Idera.SQLcompliance.Application.GUI.Forms
{
    public partial class Form_LaunchWebConsole : Form
    {
        private readonly Dictionary<string, string> _webConsoleUrls;

        public Form_LaunchWebConsole()
        {
            _webConsoleUrls = new Dictionary<string, string>();
            InitializeComponent();
            Icon = Resources.SQLcompliance_product_ico;
        }

        private void Form_LaunchWebConsole_Shown(object sender, EventArgs e)
        {
            btnRefresh_Click(sender, e);
        }

        private void btnLaunch_Click(object sender, EventArgs e)
        {
            if (lstProducts.SelectedIndex == -1)
                MessageBox.Show(@"Please select a product from the list.", Text, MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            else
            {
                Process.Start(_webConsoleUrls[lstProducts.Items[lstProducts.SelectedIndex].ToString()]);
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            Text = @"Getting list of products from CWF...";
            Cursor = Cursors.WaitCursor;
            btnLaunch.Enabled = false;

            _webConsoleUrls.Clear();
            lstProducts.Items.Clear();
            foreach (var productWebUrl in CwfHelper.Instance.GetProductWebUrls(true))
            {
                _webConsoleUrls.Add(productWebUrl.Key, productWebUrl.Value);
                lstProducts.Items.Add(productWebUrl.Key);
            }

            if (lstProducts.Items.Count > 0)
                lstProducts.SelectedIndex = 0;

            btnLaunch.Enabled = true;
            Cursor = Cursors.Default;
            Text = @"Launch Web Console";
        }
    }
}
