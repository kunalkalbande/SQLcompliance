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
    public partial class Form_SaveFileDialog : Form
    {
        private bool _applySaveEnabled;
        private string fileName;
        public Form_SaveFileDialog()
        {
            InitializeComponent();
        }

        public bool ApplySaveEnabled
        {
            get { return _applySaveEnabled; }
        }

        public string FileName
        {
            get { return fileName; }
        }

        private void txtBoxCTName_TextChanged(object sender, EventArgs e)
        {
            if (txtBoxCTName.Text.Length > 0)
            {
                btnSave.Enabled = true;
            }
            else
            {
                btnSave.Enabled = false;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            fileName = txtBoxCTName.Text;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void rbApplySaveRegulation_CheckedChanged(object sender, EventArgs e)
        {
            btnSave.Enabled = false;
            txtBoxCTName.Enabled = true;
            _applySaveEnabled = true;
        }

        private void rbApplyRegulation_CheckedChanged(object sender, EventArgs e)
        {
            btnSave.Enabled = true;
            _applySaveEnabled = false;
            txtBoxCTName.Enabled = false;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
