using Idera.SQLcompliance.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Idera.SQLcompliance.Application.GUI.Forms
{
    public partial class Form_ConfirmDefaultAuditSettings : Form
    {
        public Form_ConfirmDefaultAuditSettings()
        {
            InitializeComponent();
        }

        private void Form_ConfirmDefaultAuditSettings_Load(object sender, EventArgs e)
        {
          
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                try
                {
                    string sql = String.Format("UPDATE {0}..{1} SET isSet = 0 WHERE flagName = '" + CoreConstants.ConfirmServerDefaultAuditSettings + "'",
                       CoreConstants.RepositoryDatabase,
                       CoreConstants.RepositoryDefaultAuditSettingDialogFlags);
                    using (SqlCommand cmd = new SqlCommand(sql, Globals.Repository.Connection))
                    {
                        cmd.ExecuteScalar();
                    }

                }
                catch (Exception ex)
                {
                    //log ex
                }
            }
            this.Close();
        }
    }
}
