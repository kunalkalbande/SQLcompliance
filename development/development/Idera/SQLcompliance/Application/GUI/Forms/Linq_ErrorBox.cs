using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Idera.SQLcompliance.Application.GUI.Forms
{
    public partial class Linq_ErrorBox : Form
    {
        public Linq_ErrorBox()
        {
            InitializeComponent();
        }

        public static void Show()
        {
            Linq_ErrorBox msgBox = new Linq_ErrorBox();
            msgBox.ShowDialog();
        }

        private void okBtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void linkLbl_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(linkLbl.Text);
        }
    }
}
