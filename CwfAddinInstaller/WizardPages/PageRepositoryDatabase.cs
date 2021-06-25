using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace CwfAddinInstaller.WizardPages
{
    [WizardPageInfo("Repository Database", "Specify the name and location of SQLCM repository database.", WizardPage.RepositoryDatabase)]
    internal partial class PageRepositoryDatabase : WizardPageBase
    {
        public PageRepositoryDatabase(FormInstaller host): base(host)
        {
            InitializeComponent();
        }

        internal override void Initialize()
        {
            Host.Log("{0} - Initialized.", WizardPage.RepositoryDatabase);
            IsInitialized = true;
        }

        internal override bool DoAction(out string errorMessage, out Control invalidControl)
        {
            errorMessage = null;
            invalidControl = null;

            if (Host.CwfHelper.IsDashboardOnRemoteHost && !CwfHelper.IsInstanceNameNormalized(txtSqlServer.Text))
            {
                var normalizedInstanceName = CwfHelper.NormalizeInstanceName(txtSqlServer.Text);
                var result = MessageBox.Show(this, string.Format("Dashboard is installed on remote machine so you have to specify full instance name. Do you want to replace instance name to {0}?", normalizedInstanceName), Text, MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                if (result == DialogResult.OK)
                {
                    txtSqlServer.Text = normalizedInstanceName;
                }
                else
                {
                    errorMessage = "SQL server instance hosting SQLCM repository is invalid for remote Dashboard.";
                    return false;
                }
            }

            if (!Host.IsInputValid(txtSqlServer))
            {
                errorMessage = "SQL server instance hosting SQLCM repository not entered.";
                invalidControl = txtSqlServer;
                return false;
            }

            if (!Host.IsInputValid(txtRepositoryDatabase))
            {
                errorMessage = "SQLCM repository database name not entered.";
                invalidControl = txtRepositoryDatabase;
                return false;
            }

            txtSqlServer.Text = txtSqlServer.Text.Trim();
            txtRepositoryDatabase.Text = txtRepositoryDatabase.Text.Trim();

            if (chkSqlAuthentication.Checked)
            {
                if (!Host.IsInputValid(txtSqlUser))
                {
                    errorMessage = "SQL server user name not entered.";
                    invalidControl = txtSqlUser;
                    return false;
                }

                if (!Host.IsInputValid(txtSqlPassword))
                {
                    errorMessage = "SQL server user password not entered.";
                    invalidControl = txtSqlPassword;
                    return false;
                }

                txtSqlUser.Text = txtSqlUser.Text.Trim();
            }

            
            if (!CheckServerConnection(out errorMessage))
            {
                errorMessage = string.Format("Failed to connect to SQL server '{1}'.{0}Error Message: {2}", Environment.NewLine, txtSqlServer.Text.Trim(), errorMessage);
                Host.Log(errorMessage);
                return false;
            }
            

            Host.CwfHelper.RepositoryInstance = txtSqlServer.Text;
            Host.CwfHelper.RepositoryDatabase = txtRepositoryDatabase.Text;

            Host.Log("Repository SQL Server: {0}", Host.CwfHelper.RepositoryInstance);
            Host.Log("Repository Database: {0}", Host.CwfHelper.RepositoryDatabase);
            Host.Log("Using SQL Server Authentication: {0}", chkSqlAuthentication.Checked);
            Host.Log("SQL Server User Name: {0}", txtSqlUser.Text);
            Host.Log("SQL Server Password: {0}", FormInstaller.NoPassowrdlogging);

            return true;
        }

        private bool CheckServerConnection(out string errorMessage)
        {
            errorMessage = string.Empty;
            try
            {
                using (var connection = new SqlConnection())
                {
                    connection.ConnectionString = chkSqlAuthentication.Checked 
                        ? string.Format("Server={0};Database=master;User Id={1};Password={2};", txtSqlServer.Text, txtSqlUser.Text, txtSqlPassword.Text)
                        : string.Format("Server={0};Database=master;Integrated Security=SSPI;", txtSqlServer.Text);
                    connection.Open();
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandType = CommandType.Text;
                        command.CommandText = string.Format("SELECT COUNT(*) AS 'MasterDatabaseCount'  FROM sys.databases WHERE name = 'master'");
                        var result = command.ExecuteScalar();
                        if (result == DBNull.Value)
                            return false;

                        return Convert.ToInt32(result) >= 1;
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }

            return false;
        }

        private void chkSqlAuthentication_CheckedChanged(object sender, EventArgs e)
        {
            var isSqlAuthentication = chkSqlAuthentication.Checked;
            txtSqlUser.Enabled = txtSqlPassword.Enabled = isSqlAuthentication;
            if (isSqlAuthentication)
            {
                txtSqlUser.SelectAll();
                txtSqlUser.Focus();
            }
        }

        private void btnSelectServer_Click(object sender, EventArgs e)
        {
            var selectServer = new FormServers(Host, txtSqlServer.Text);
            if (selectServer.ShowDialog(this) == DialogResult.OK)
                txtSqlServer.Text = selectServer.SelectedServerInstance;
        }
    }
}
