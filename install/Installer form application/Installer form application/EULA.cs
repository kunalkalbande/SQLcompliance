using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Installer_form_application
{
    public partial class EULA : Form
    {
        Form screenObject;

        public EULA(Form screenObj)
        {
            InitializeComponent();
            MinimizeBox = false;
            MaximizeBox = false;
            screenObject = screenObj;
        }

        private void EULA_Load(object sender, EventArgs e)
        {
            checkBoxAccept.Checked = properties.AGREETOLICENSE;
            String path = Directory.GetCurrentDirectory() + "\\Idera - Software License Agreement.rtf";
            richTextBoxEULA.LoadFile(Directory.GetCurrentDirectory() + "\\Idera - Software License Agreement.rtf");
        }

        private void buttonBack_Click(object sender, EventArgs e)
        {
            properties.AGREETOLICENSE = checkBoxAccept.Checked;
            this.Hide();
            screenObject.Show();
        }

        private void buttonNext_Click(object sender, EventArgs e)
        {
            properties.AGREETOLICENSE = checkBoxAccept.Checked;
            this.Hide();
            Credentials cred = new Credentials(this);
            cred.Show();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            if (DialogResult.Yes == MessageBox.Show("Do you really want to exit?", "Exit", MessageBoxButtons.YesNo))
            {
                Application.Exit();
            }
        }

        private void checkBoxAccept_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxAccept.Checked)
            {
                buttonNext.Enabled = true;
            }
            else
            {
                buttonNext.Enabled = false;
            }
        }

        private void labelHeading_Click(object sender, EventArgs e)
        {

        }

    }
}
