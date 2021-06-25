using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Idera.SQLcompliance.Application.GUI.Controls;
using Idera.SQLcompliance.Application.GUI.Properties;
using Idera.SQLcompliance.Core.Agent;

namespace Idera.SQLcompliance.Application.GUI.Forms
{
    public partial class Form_PermissionsCheck : Form
    {
        private readonly Queue<ServerRecord> _serversToCheck;

        private int _totalServers;
        private int _passedServers;
        private int _failedServers;

        private int _totalFailedChecks;
        private StringBuilder _totalResolutionStepsRtf;

        #region constructors

        public Form_PermissionsCheck()
        {
            _serversToCheck = new Queue<ServerRecord>();
            _totalResolutionStepsRtf = new StringBuilder();

            InitializeComponent();

            Icon = Resources.SQLcompliance_product_ico;
        }

        #endregion

        private void Form_PermissionsCheck_Shown(object sender, EventArgs e)
        {
            permissionsCheck.IsReCheckAllowed = false;
            permissionsCheck.IsResolutionStepsShownOnFailure = false;
            progStatusIndivisual.Maximum = permissionsCheck.TotalChecks;
            Reset();

            // populate list of audited servers
            lstAuditedSqlServers.Items.Clear();
            foreach (ServerRecord server in ServerRecord.GetServers(Globals.Repository.Connection, false, true))
                lstAuditedSqlServers.Items.Add(server, false);
        }

        private void Form_PermissionsCheck_FormClosing(object sender, FormClosingEventArgs e)
        {
            CancelPermissionsCheck();
        }

        private void btnCheckPermissions_Click(object sender, EventArgs e)
        {
            if (lstAuditedSqlServers.CheckedItems.Count == 0)
            {
                lstAuditedSqlServers.Focus();
                return;
            }

            Reset();

            // disable user input
            lstAuditedSqlServers.Enabled = false;
            btnCheckPermissions.Enabled = false;
            btnCancel.Enabled = true;

            // enqueue servers for checking
            _serversToCheck.Clear();
            foreach (ServerRecord server in lstAuditedSqlServers.CheckedItems)
                _serversToCheck.Enqueue(server);

            _totalServers = _serversToCheck.Count;
            progStatusTotal.Maximum = _totalServers;
            Text = @"Permissions check in progress. Please Wait...";

            CheckPermission();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Text = @"Permissions check in progress. Cancelling, please wait...";
            btnCancel.Enabled = false;

            CancelPermissionsCheck();
            Reset();

            Text = @"Permissions Check";

            DialogResult = DialogResult.None;
        }

        #region private methods

        private void Reset()
        {
            _totalServers = 0;
            _passedServers = 0;
            _failedServers = 0;

            _totalFailedChecks = 0;
            _totalResolutionStepsRtf = new StringBuilder();

            permissionsCheck.Reset();
            btnCheckPermissions.Enabled = false;
            btnCancel.Enabled = false;

            lblProgressTotal.Text = @"Total Servers: 0, Passed Servers: 0, Failed Servers: 0";
            lblStatsTotal.Text = @"0 of 0";
            progStatusTotal.Value = 0;
            progStatusTotal.Maximum = 100;

            lblProgressIndivisual.Text = @"Idle";
            lblStatsIndivisual.Text = string.Format("0 of {0}", permissionsCheck.TotalChecks);
            progStatusIndivisual.Value = 0;
        }

        private void CheckPermission()
        {
            ServerRecord serverToCheck = _serversToCheck.Dequeue();

            progStatusIndivisual.Value = 0;
            lblProgressIndivisual.Text = string.Format("Checking permissions for {0}.", serverToCheck.Instance);

            // update status for all servers
            lblProgressTotal.Text = string.Format("Total Servers: {0}, Passed Servers: {1}, Failed Servers: {2}", _totalServers, _passedServers, _failedServers);
            lblStatsTotal.Text = string.Format("{0} of {1}", _passedServers + _failedServers, _totalServers);

            permissionsCheck.StartPermissionsCheck(serverToCheck);
        }

        private void CancelPermissionsCheck()
        {
            if (permissionsCheck.CheckStatus == PermissionsCheck.Status.InProgress)
                permissionsCheck.StopPermissionsCheck();
        }

        #endregion

        private void permissionsCheck_OnPermissionsCheckCompleted(PermissionsCheck.Status checkStatus)
        {
            // update status if permission check is not cancelled
            if (permissionsCheck.CheckStatus != PermissionsCheck.Status.Cancelled)
            {
                _passedServers += checkStatus == PermissionsCheck.Status.Passesd ? 1 : 0;
                _failedServers += checkStatus == PermissionsCheck.Status.Failed ? 1 : 0;

                lblProgressTotal.Text = string.Format("Total Servers: {0}, Passed Servers: {1}, Failed Servers: {2}", _totalServers, _passedServers, _failedServers);
                lblStatsTotal.Text = string.Format("{0} of {1}", _passedServers + _failedServers, _totalServers);

                progStatusTotal.Value += 1;
            }

            _totalResolutionStepsRtf.Append(" ");
            _totalResolutionStepsRtf.Append(permissionsCheck.ResolutionStepsRtf);
            
            if (checkStatus == PermissionsCheck.Status.Cancelled || _serversToCheck.Count == 0)
            {
                lblProgressIndivisual.Text = @"Idle";
                Text = @"Permissions Check";
                
                // enable user input
                lstAuditedSqlServers.Enabled = true;
                btnCheckPermissions.Enabled = true;
                btnCancel.Enabled = false;

                if (chkShowResolutionSteps.Checked && checkStatus != PermissionsCheck.Status.Cancelled)
                        permissionsCheck.ShowResolutionSteps(_totalFailedChecks, _totalResolutionStepsRtf.ToString(), false);
            }
            else
                CheckPermission();
        }

        private void permissionsCheck_OnPermissionCheckComplete(PermissionsCheck.Status status)
        {
            // update indivisual server's permission check status
            lblStatsIndivisual.Text = string.Format("{0} of {1}",
                                                     permissionsCheck.PassedChecks + permissionsCheck.FailedChecks, 
                                                     permissionsCheck.TotalChecks);
            _totalFailedChecks += status == PermissionsCheck.Status.Failed ? 1 : 0;
            progStatusIndivisual.Value += 1;
        }

        private void lstAuditedSqlServers_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (lstAuditedSqlServers.CheckedItems.Count == 0 && e.NewValue == CheckState.Checked)
                btnCheckPermissions.Enabled = true;
            else if (lstAuditedSqlServers.CheckedItems.Count == 1 && e.NewValue == CheckState.Unchecked)
                btnCheckPermissions.Enabled = false;
            else
                btnCheckPermissions.Enabled = true;
        }
    }
}