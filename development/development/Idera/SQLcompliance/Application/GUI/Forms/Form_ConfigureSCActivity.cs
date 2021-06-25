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
    public partial class Form_ConfigureSCActivity : Form
    {
        private int _dbId;
        private int _srvId;
        private int sensitiveColumnActivity = 0;
        public Form_ConfigureSCActivity(int srvId, int dbId)
        {
            InitializeComponent();
            _srvId = srvId;
            _dbId = dbId;
            SetSensitiveColumnActivity();
        }
        private void SetSensitiveColumnActivity()
        {
            SqlConnection connection = Globals.Repository.Connection;
            SQLHelpers.CheckConnection(connection);
            string cmdstr = string.Empty;
            cmdstr = string.Format(@"select auditSensitiveColumnActivity from Databases d where d.dbId = {0}", _dbId);

            using (SqlCommand cmd = new SqlCommand(cmdstr, connection))
            {
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        sensitiveColumnActivity = Convert.ToInt32(reader[0]);
                    }
                }
            }
            if (sensitiveColumnActivity == 0)
                this.rdSelectOnly.Checked = true;
            else if (sensitiveColumnActivity == 1)
                this.rdSelectAndDML.Checked = true;
            else this.rdAllActivity.Checked = true;

        }


        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (rdSelectAndDML.Checked)
                    sensitiveColumnActivity = 1;
                else if (rdAllActivity.Checked)
                    sensitiveColumnActivity = 2;
                else if (rdSelectOnly.Checked)
                    sensitiveColumnActivity = 0;
                string query = string.Format("Update {0} set auditSensitiveColumnActivity = @scActivity, auditSensitiveColumnActivityDataset = @scActivity where dbId = {1} and srvId = {2} ",
                    CoreConstants.RepositoryDatabaseTable, _dbId, _srvId);
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = Globals.Repository.Connection;
                    cmd.CommandText = query;
                    cmd.Parameters.AddWithValue("@scActivity", sensitiveColumnActivity);
                    cmd.ExecuteNonQuery();
                }
                MessageBox.Show(this, "Successfully saved the Sensitive Column Activity.", "Sensitive Column Activity", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Close();

            }
            catch (Exception exp)
            {
                MessageBox.Show("Error: "+exp.Message);
            }

        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        
    }
}
