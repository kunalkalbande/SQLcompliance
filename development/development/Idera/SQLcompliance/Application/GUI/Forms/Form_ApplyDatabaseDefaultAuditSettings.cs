using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Idera.SQLcompliance.Application.GUI.Controls;
using Idera.SQLcompliance.Application.GUI.Helper;
using Idera.SQLcompliance.Core;
using Idera.SQLcompliance.Core.Collector;
using Microsoft.Win32;
using Idera.SQLcompliance.Application.GUI.SQL;
using Idera.SQLcompliance.Core.Agent;
using Idera.SQLcompliance.Core.Remoting;
using Idera.SQLcompliance.Application.GUI.Properties;
using System.Transactions;

namespace Idera.SQLcompliance.Application.GUI.Forms
{
    public partial class Form_ApplyDatabaseDefaultAuditSettings : Form
    {
        private string ReadServerFromRegistry()
        {
            RegistryKey rk = null;
            RegistryKey rks = null;
            string serverInstance = null;

            try
            {
                rk = Registry.LocalMachine;
                rks = rk.CreateSubKey(CoreConstants.CollectionService_RegKey, RegistryKeyPermissionCheck.ReadSubTree);

                serverInstance = (string)rks.GetValue(CoreConstants.CollectionService_RegVal_ServerInstance, null);
            }
            catch (Exception)
            {
            }
            finally
            {
                if (rks != null) rks.Close();
                if (rk != null) rk.Close();
            }
            if (serverInstance != null)
                return serverInstance.ToUpper();
            else
                return null;
        }
        public Form_ApplyDatabaseDefaultAuditSettings(DefaultSettings defaultSettings)
        {
            InitializeComponent();
            this.Icon = Resources.SQLcompliance_product_ico;
            listView1.View = View.Details;
            ColumnHeader columnHeader = new ColumnHeader();
            columnHeader.Width = listView1.Width;
            listView1.Columns.Add(columnHeader);
            listView1.HeaderStyle = ColumnHeaderStyle.None;
            foreach (ListViewItem listViewItem in defaultSettings.GetCheckedDatabases().Items)
            {
              
                listView1.Items.Add((ListViewItem)listViewItem.Clone());
            }
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string queryDefaultSettings = String.Format("select * from {0}..{1}", CoreConstants.RepositoryDatabase, CoreConstants.RepositoryDefaultDatabaseSettings);
            string serverDefaultSettings = String.Format("select * from {0}..{1}", CoreConstants.RepositoryDatabase, CoreConstants.DefaultServerPropertise);


            try
            {
                using (TransactionScope transactionScope = new TransactionScope())
                {
                    foreach (ListViewItem item in listView1.Items)
                    {
                        var serverName = item.Text.Split(',')[0];
                        string query = string.Format("update {0}..{1}", CoreConstants.RepositoryDatabase, CoreConstants.RepositoryDatabaseTable);
                        query = query + String.Format(" set auditDDL=@auditDDL,auditSecurity= @auditSecurity, auditAdmin =@auditAdmin," +
                     "auditDML =@auditDML,auditSELECT = @auditSelect , auditFailures= @auditFailure , auditCaptureSQL = @auditCaptureSql," +
                     "auditCaptureTrans = @auditCaptureTrans,auditCaptureDDL =@auditCaptureDDL , auditUsersList = @auditUserlist,auditPrivUsersList=@auditPrivList," +
                     "auditUserAll=@userAll , auditUserLogins=@userlogin,auditUserLogouts=@userlogout,auditUserFailedLogins=@userfailedlog," +
                     "auditUserDDL= @userDDL , auditUserSecurity=@userSecurity, auditUserAdmin=@userAdmin,auditUserDML = @userDML," +
                     "auditUserSELECT = @userSelect,auditUserUDE = @userUDE,auditUserFailures=@userFailure,auditUserCaptureSQL=@usercapturesql," +
                     "auditUserCaptureTrans=@userCaptureTrans , auditUserCaptureDDL =@userCaptureDDL where name = '{0}' and srvInstance='{1}' and srvId={2}", item.Text.Substring(item.Text.LastIndexOf(',') + 1), item.Text.Split(',')[0], (int)item.Tag);



                        var dbSettings = DefaultAuditSettingsHelper.GetDBAuditSettings(serverName);

                        using (SqlCommand cmd = new SqlCommand())
                        {
                            cmd.CommandText = query;
                            cmd.Connection = Globals.Repository.Connection;
                            cmd.Parameters.AddWithValue("@auditDDL", dbSettings.AuditDDL);
                            cmd.Parameters.AddWithValue("@auditSecurity", dbSettings.AuditSecurity);
                            cmd.Parameters.AddWithValue("@auditAdmin", dbSettings.AuditAdmin);
                            cmd.Parameters.AddWithValue("@auditDML", dbSettings.AuditDML);
                            cmd.Parameters.AddWithValue("@auditSelect", dbSettings.AuditSELECT);
                            cmd.Parameters.AddWithValue("@auditFailure", dbSettings.AuditAccessCheck);
                            cmd.Parameters.AddWithValue("@auditCaptureSql", dbSettings.AuditCaptureSQL);
                            cmd.Parameters.AddWithValue("@auditCaptureTrans", dbSettings.AuditCaptureTrans);
                            cmd.Parameters.AddWithValue("@auditCaptureDDL", dbSettings.AuditCaptureDDL);
                            cmd.Parameters.AddWithValue("@auditUserlist", dbSettings.AuditUsersList);
                            cmd.Parameters.AddWithValue("@auditPrivList", dbSettings.AuditPrivUsersList);
                            cmd.Parameters.AddWithValue("@userAll", dbSettings.AuditUserAll);
                            cmd.Parameters.AddWithValue("@userlogin", dbSettings.AuditUserLogins);
                            cmd.Parameters.AddWithValue("@userlogout", dbSettings.AuditUserLogouts);
                            cmd.Parameters.AddWithValue("@userfailedlog", dbSettings.AuditUserFailedLogins);
                            cmd.Parameters.AddWithValue("@userDDL", dbSettings.AuditUserDDL);
                            cmd.Parameters.AddWithValue("@userSecurity", dbSettings.AuditUserSecurity);
                            cmd.Parameters.AddWithValue("@userAdmin", dbSettings.AuditUserAdmin);
                            cmd.Parameters.AddWithValue("@userDML", dbSettings.AuditUserDML);
                            cmd.Parameters.AddWithValue("@userSelect", dbSettings.AuditUserSELECT);
                            cmd.Parameters.AddWithValue("@userUDE", dbSettings.AuditUserUDE);
                            cmd.Parameters.AddWithValue("@userFailure", dbSettings.AuditUserAccessCheck);
                            cmd.Parameters.AddWithValue("@usercapturesql", dbSettings.AuditUserCaptureSQL);
                            cmd.Parameters.AddWithValue("@userCaptureTrans", dbSettings.AuditUserCaptureTrans);
                            cmd.Parameters.AddWithValue("@userCaptureDDL", dbSettings.AuditUserCaptureDDL);
                            cmd.ExecuteNonQuery();
                        }
                    }//for

                    transactionScope.Complete();
                    MessageBox.Show("The settings have been applied to the selected databases.");
                    Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("" + ex);
            }

        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var auditedActivity = string.Empty;
            var trustedUserLogins = string.Empty;
            var trustedUSerRoles = string.Empty;
            var privUserLogins = string.Empty;
            var privUserRoles = string.Empty;
            var privUserActivity = string.Empty;


            string selectQuery = String.Format("select * from {0}..{1}", CoreConstants.RepositoryDatabase, CoreConstants.RepositoryDefaultDatabaseSettings);
            try
            {
                using (SqlCommand command = new SqlCommand(selectQuery, Globals.Repository.Connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        reader.Read();
                        UserList defaultPrivilegedUserList = new UserList(SQLHelpers.GetString(reader, "auditPrivUsersList"));
                        UserList defaultTrustedUserList = new UserList(SQLHelpers.GetString(reader, "auditUsersList"));

                        foreach (var prvLogin in defaultPrivilegedUserList.Logins)
                        {
                            privUserLogins += ", " + prvLogin.Name;
                        }

                        foreach (var prvServerRole in defaultPrivilegedUserList.ServerRoles)
                        {
                            privUserRoles += ", " + prvServerRole.Name;
                        }

                        foreach (var trustedLogin in defaultTrustedUserList.Logins)
                        {
                            trustedUserLogins += ", " + trustedLogin.Name;
                        }

                        foreach (var trustedServerRole in defaultTrustedUserList.ServerRoles)
                        {
                            trustedUSerRoles += ", " + trustedServerRole.Name;
                        }
  
                        if (SQLHelpers.GetBool(reader, "auditDDL"))
                            auditedActivity += ", Database Definition(DDL)";
                        if (SQLHelpers.GetBool(reader, "auditSecurity"))
                            auditedActivity += ", Security Changes";
                        if (SQLHelpers.GetBool(reader, "auditAdmin"))
                            auditedActivity += ", Administrative Actions";
                        if (SQLHelpers.GetBool(reader, "auditDML"))
                            auditedActivity += ", Database Modification";
                        if (SQLHelpers.GetBool(reader, "auditSelect"))
                            auditedActivity += ", Database SELECT Operations";
                        if (SQLHelpers.GetBool(reader, "auditCaptureSql"))
                            auditedActivity += ", Capture SQL Statements for DML and SELECT activities";
                        if (SQLHelpers.GetBool(reader, "auditCaptureTrans"))
                            auditedActivity += ", Capture Transaction Status for DML Activity";
                        if (SQLHelpers.GetBool(reader, "auditCaptureDDL"))
                            auditedActivity += ", Capture SQL statements for DDL and Security Changes";

                        switch (SQLHelpers.GetByteToInt(reader, "auditFailures"))
                        {
                            case 0:
                                auditedActivity += ", Passed";
                                break;
                            case 2:
                                auditedActivity += ", Failed";
                                break;
                        }

                        if (SQLHelpers.GetBool(reader, "auditUserAll"))
                            privUserActivity = ", Audit all activities done by Privileged Users";
                        else
                            privUserActivity = ", Audit selected activities done by Privileged Users";

                        
                        if (!string.IsNullOrEmpty(auditedActivity))
                            auditedActivity = "Audited Activities: " + auditedActivity.Substring(2) + "!";
                        if (!string.IsNullOrEmpty(trustedUserLogins))
                            trustedUserLogins = "Trusted User Logins: " + trustedUserLogins.Substring(2) + "!";
                        if (!string.IsNullOrEmpty(trustedUSerRoles))
                            trustedUSerRoles = "Trusted User Roles: " + trustedUSerRoles.Substring(2) + "!";
                        if (!string.IsNullOrEmpty(privUserLogins))
                            privUserLogins = "Privileged User Logins: " + privUserLogins.Substring(2) + "!";
                        if (!string.IsNullOrEmpty(privUserRoles))
                            privUserRoles = "Privileged User Roles: " + privUserRoles.Substring(2) + "!";
                        if (!string.IsNullOrEmpty(privUserActivity))
                            privUserActivity = "Privileged User Audit Activity: " + privUserActivity.Substring(2) + "!";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("" + ex);
            }

            Form_DisplayServerDefaultAuditSettings frm = new Form_DisplayServerDefaultAuditSettings(" databases.",auditedActivity + trustedUserLogins + trustedUSerRoles + privUserLogins + privUserRoles + privUserActivity);
            frm.StartPosition = FormStartPosition.CenterParent;
            frm.ShowDialog(this);
        }
    }
}
