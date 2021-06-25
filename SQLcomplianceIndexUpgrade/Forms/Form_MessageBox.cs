using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SQLcomplianceIndexUpgrade.Forms
{
    public partial class Form_MessageBox : Form
    {
        public Form_MessageBox()
        {
            InitializeComponent();
        }

        private void lnkMessage_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            lnkMessage.LinkVisited = true;
            System.Diagnostics.Process.Start("http://wiki.idera.com/x/WBSzBw");
        }

        private void ok_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
