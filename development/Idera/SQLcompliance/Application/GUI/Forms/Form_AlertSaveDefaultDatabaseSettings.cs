using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Idera.SQLcompliance.Core;

namespace Idera.SQLcompliance.Application.GUI.Forms
{
    public partial class Form_AlertSaveDefaultDatabaseSettings : Form
    {
        public Form_AlertSaveDefaultDatabaseSettings()
        {
            InitializeComponent();
            FormClosed += OnClose;
        }


        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void OnClose(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                try
                {
                    string query = string.Format("update {0}..{1} set isSet=0 where flagName='CONFIRM_DATABASE_DEFAULT_AUDIT_SETTINGS'", CoreConstants.RepositoryDatabase, CoreConstants.RepositoryDefaultAuditSettingDialogFlags);
                    using (SqlCommand cmd = new SqlCommand(query, Globals.Repository.Connection))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("" + ex);
                }
            }
        }
    }
}
