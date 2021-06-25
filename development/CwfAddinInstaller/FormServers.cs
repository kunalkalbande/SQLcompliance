using System;
using System.Data;
using System.Data.Sql;
using System.Windows.Forms;

namespace CwfAddinInstaller
{
    internal partial class FormServers : Form
    {
        private readonly FormInstaller _host;

        public FormServers(FormInstaller host, string selectedInstance = "")
        {
            _host = host;
            InitializeComponent();

            SelectedServerInstance = selectedInstance;
            Icon = _host.Icon;
        }

        public string SelectedServerInstance { get; private set; }

        private void FormServers_Shown(object sender, EventArgs e)
        {
            btnRefresh_Click(sender, e);
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            var title = Text;

            Cursor = Cursors.WaitCursor;
            Text = @"Getting list of SQL server instances...";
            btnOk.Enabled = false;
            lstServers.Items.Clear();
            Application.DoEvents();

            _host.Log("Getting list of SQL server instances...");
            var table = SqlDataSourceEnumerator.Instance.GetDataSources();
            var count = 0;
            
            foreach (DataRow dr in table.Rows)
            {
                var serverName = dr["ServerName"];
                var instanceName = dr["InstanceName"];
                count += 1;

                var name = instanceName.Equals(DBNull.Value)
                           ? serverName.ToString()
                           : string.Format("{0}\\{1}", serverName, instanceName);

                var index = lstServers.Items.Add(name);

                // select instance
                if (!string.IsNullOrEmpty(SelectedServerInstance) && 
                    SelectedServerInstance.Equals(name, StringComparison.InvariantCultureIgnoreCase))
                    lstServers.SelectedIndex = index;
            }
            _host.Log("SQL Server Instances detected: {0}", count);

            Cursor = Cursors.Default;
            Text = title;
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

        private void lstServers_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectedServerInstance = lstServers.SelectedIndex > -1 ?
                                     lstServers.Items[lstServers.SelectedIndex].ToString() :
                                     string.Empty;
            btnOk.Enabled = lstServers.SelectedIndex > -1;
        }
    }
}
