using Idera.SQLcompliance.Application.GUI.Properties;
using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace Idera.SQLcompliance.Application.GUI.Forms
{
    public partial class Form_MessageBox : Form
    {
        private readonly string _link;

        public Form_MessageBox(Form owner, string message, string title, string link = "")
        {
            _link = link;
            InitializeComponent();

            Owner = owner;
            Icon = Resources.SQLcompliance_product_ico;
            Text = title;

            // automatically make link
            lnkMessage.Text = message;
            if (message.Contains(_link))
                lnkMessage.LinkArea = new LinkArea(message.IndexOf(_link, StringComparison.InvariantCultureIgnoreCase), _link.Length);
        }

        private void lnkMessage_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (!string.IsNullOrEmpty(_link))
                Process.Start(_link);
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
