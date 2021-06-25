using System;
using System.Drawing;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Idera.SQLcompliance.Application.GUI.Forms;
using Idera.SQLcompliance.Application.GUI.Helper;
using Idera.SQLcompliance.Core;
using Idera.SQLcompliance.Core.Agent;
using ManagementConsoleRequest = Idera.SQLcompliance.Core.Collector.ManagementConsoleRequest;

namespace Idera.SQLcompliance.Application.GUI.Controls
{
    public partial class PermissionsCheck : UserControl
    {
        public enum Status : byte
        {
            Waiting,
            InProgress,
            Passesd,
            Failed,
            Cancelled
        }

        private volatile Status _checkStatus;
        private Thread _permissionsChecker;
        private volatile bool _isResolutionStepsShownOnFailure;
        private volatile bool _blockUserActivity;
        private ServerRecord _server;

        private readonly int _totalChecks;
        private int _passedChecks;
        private int _failedChecks;
        private string _resolutionStepsRtf;

        public delegate void PermissionsCheckCompleted(Status checkStatus);
        public event PermissionsCheckCompleted OnPermissionsCheckCompleted;

        public delegate void PermissionCheckComplete(Status checkStatus);
        public event PermissionCheckComplete OnPermissionCheckComplete;

        #region constructor

        public PermissionsCheck()
        {
            _checkStatus = Status.Waiting;
            _blockUserActivity = false;
            _permissionsChecker = null;
            _server = null;
            _isResolutionStepsShownOnFailure = true;
            _resolutionStepsRtf = string.Empty;

            InitializeComponent();

            _totalChecks = lstChecks.Items.Count;
        }

        #endregion

        #region properties

        public bool IsReCheckAllowed
        {
            get { return btnReCheck.Visible; }
            set { btnReCheck.Visible = value; }
        }

        public bool IsResolutionStepsShownOnFailure
        {
            get { return _isResolutionStepsShownOnFailure; }
            set { _isResolutionStepsShownOnFailure = value; }
        }

        public bool IsUserActivityBlocked
        {
            get { return _blockUserActivity; }
        }

        public Status CheckStatus
        {
            get { return _checkStatus; }
        }

        public int TotalChecks
        {
            get { return _totalChecks; }
        }

        public int PassedChecks
        {
            get { return _passedChecks; }
        }

        public int FailedChecks
        {
            get { return _failedChecks; }
        }

        public string ResolutionStepsRtf
        {
            get { return _resolutionStepsRtf; }
        }

        #endregion

        #region private methods

        private void SetStatus(int index, Status status, int total, ref int passed, ref int failed)
        {
            switch (status)
            {
                case Status.Waiting:
                    lstChecks.Items[index].UseItemStyleForSubItems = false;
                    lstChecks.Items[index].ImageKey = @"(none)";
                    lstChecks.Items[index].SubItems[2].Text = @"Waiting";
                    lstChecks.Items[index].SubItems[2].ForeColor = Color.Black;
                    break;
                case Status.InProgress:
                    lstChecks.Items[index].UseItemStyleForSubItems = false;
                    lstChecks.Items[index].ImageKey = @"imgInProgress";
                    lstChecks.Items[index].SubItems[2].Text = @"In Progress";
                    lstChecks.Items[index].SubItems[2].ForeColor = Color.Blue;
                    break;
                case Status.Passesd:
                    lstChecks.Items[index].UseItemStyleForSubItems = false;
                    lstChecks.Items[index].ImageKey = @"imgPass";
                    lstChecks.Items[index].SubItems[2].Text = @"Passed";
                    lstChecks.Items[index].SubItems[2].ForeColor = Color.Green;
                    passed += 1;
                    break;
                case Status.Failed:
                    lstChecks.Items[index].UseItemStyleForSubItems = false;
                    lstChecks.Items[index].ImageKey = @"imgFail";
                    lstChecks.Items[index].SubItems[2].Text = @"Failed";
                    lstChecks.Items[index].SubItems[2].ForeColor = Color.Red;
                    failed += 1;
                    break;
            }

            lblProgress.Text = string.Format("{0}. Total {1}, Passed {2}, Failed {3}", index == total - 1 ? "Operation Complete" : "Checking permissions, please wait", total, passed, failed);
        }

        private void PermissionsChecker(object server)
        {
            if (server == null)
                return;

            try
            {
                // enable logging to event logs
                var eventLoggingEnabled = ErrorLog.Instance.LogToEventLog;
                if (!eventLoggingEnabled)
                    ErrorLog.Instance.LogToEventLog = true;

                _checkStatus = Status.InProgress;
                _blockUserActivity = false;
                _resolutionStepsRtf = string.Empty;

                var serverToCheck = (ServerRecord) server;

                // disable user input
                btnReCheck.Enabled = false;
                Cursor = Cursors.WaitCursor;

                int index;
                _passedChecks = 0;
                _failedChecks = 0;

                // reset all checks to waiting
                for (index = 0; index < lstChecks.Items.Count; index += 1)
                    SetStatus(index, Status.Waiting, _totalChecks, ref _passedChecks, ref _failedChecks);

                var collectionServiceRequest = GUIRemoteObjectsProvider.CollectorManagementConsoleRequest();
                var agentServiceRequest = GUIRemoteObjectsProvider.AgentManagementConsoleRequest(serverToCheck.AgentServer, serverToCheck.AgentPort);

                Status status;
                var resolutionStepsBuilder = new StringBuilder();
                string resolutionSteps;
                index = 0;

                resolutionStepsBuilder.AppendFormat("\\ul\\b\\cf3 Permissions Check for SQL Server Instance {0}: \\cf1\\b0\\ulnone\\line ", serverToCheck.Instance.Replace("\\", "\\\\"));
                resolutionStepsBuilder.Append("\\par");

                // CHECK1: Collection Service has rights to the repository databases.
                SetStatus(index, Status.InProgress, _totalChecks, ref _passedChecks, ref _failedChecks);
                try
                {
                    resolutionSteps = collectionServiceRequest.HasRoghtsToRepository();
                    if (string.IsNullOrEmpty(resolutionSteps))
                        status = Status.Passesd;
                    else
                    {
                        status = Status.Failed;
                        resolutionStepsBuilder.AppendFormat("\\b {0} \\b0\\line ", CoreConstants.PermissionCheck1.Replace("\\", "\\\\"));
                        resolutionStepsBuilder.Append(resolutionSteps);
                        resolutionStepsBuilder.Append("\\par");
                    }
                }
                catch (Exception ex)
                {
                    status = Status.Failed;
                    resolutionStepsBuilder.AppendFormat("\\b {0} \\b0\\line ", CoreConstants.PermissionCheck1.Replace("\\", "\\\\"));

                    if (ex is SocketException)
                        resolutionStepsBuilder.Append("A connection could not be made to Collection Server. \\line ");

                    resolutionStepsBuilder.AppendFormat("ERROR: {0} \\line ", ex.Message);
                    resolutionStepsBuilder.Append("\\par");
                    ErrorLog.Instance.Write(ErrorLog.Level.UltraDebug, CoreConstants.PermissionCheck1, ex.Message, ErrorLog.Severity.Informational);
                }
                SetStatus(index++, status, _totalChecks, ref _passedChecks, ref _failedChecks);
                if (OnPermissionCheckComplete != null)
                    OnPermissionCheckComplete(status);

                // CHECK2: Collection Service has rights to read registry HKLM\Software\Idera\SQLCM.
                SetStatus(index, Status.InProgress, _totalChecks, ref _passedChecks, ref _failedChecks);
                try
                {
                    resolutionSteps = collectionServiceRequest.HasRightToReadRegistry();
                    if (string.IsNullOrEmpty(resolutionSteps))
                        status = Status.Passesd;
                    else
                    {
                        status = Status.Failed;
                        resolutionStepsBuilder.AppendFormat("\\b {0} \\b0\\line ", CoreConstants.PermissionCheck2.Replace("\\", "\\\\"));
                        resolutionStepsBuilder.Append(resolutionSteps);
                        resolutionStepsBuilder.Append("\\par");
                    }
                }
                catch (Exception ex)
                {
                    status = Status.Failed;
                    resolutionStepsBuilder.AppendFormat("\\b {0} \\b0\\line ", CoreConstants.PermissionCheck2.Replace("\\", "\\\\"));

                    if (ex is SocketException)
                        resolutionStepsBuilder.Append("A connection could not be made to Collection Server. \\line ");

                    resolutionStepsBuilder.AppendFormat("ERROR: {0} \\line ", ex.Message);
                    resolutionStepsBuilder.Append("\\par");
                    ErrorLog.Instance.Write(ErrorLog.Level.UltraDebug, CoreConstants.PermissionCheck2, ex.Message, ErrorLog.Severity.Informational);
                }
                SetStatus(index++, status, _totalChecks, ref _passedChecks, ref _failedChecks);
                if (OnPermissionCheckComplete != null)
                    OnPermissionCheckComplete(status);

                // CHECK3: Collection Service has permissions to collection trace directory.
                SetStatus(index, Status.InProgress, _totalChecks, ref _passedChecks, ref _failedChecks);
                try
                {
                    resolutionSteps = collectionServiceRequest.HasPermissionToTraceDirectory();
                    if (string.IsNullOrEmpty(resolutionSteps))
                        status = Status.Passesd;
                    else
                    {
                        status = Status.Failed;
                        resolutionStepsBuilder.AppendFormat("\\b {0} \\b0\\line ", CoreConstants.PermissionCheck3.Replace("\\", "\\\\"));
                        resolutionStepsBuilder.Append(resolutionSteps);
                        resolutionStepsBuilder.Append("\\par");
                    }
                }
                catch (Exception ex)
                {
                    status = Status.Failed;
                    resolutionStepsBuilder.AppendFormat("\\b {0} \\b0\\line ", CoreConstants.PermissionCheck3.Replace("\\", "\\\\"));

                    if (ex is SocketException)
                        resolutionStepsBuilder.Append("A connection could not be made to Collection Server. \\line ");

                    resolutionStepsBuilder.AppendFormat("ERROR: {0} \\line ", ex.Message);
                    resolutionStepsBuilder.Append("\\par");
                    ErrorLog.Instance.Write(ErrorLog.Level.UltraDebug, CoreConstants.PermissionCheck3, ex.Message, ErrorLog.Severity.Informational);
                }
                SetStatus(index++, status, _totalChecks, ref _passedChecks, ref _failedChecks);
                if (OnPermissionCheckComplete != null)
                    OnPermissionCheckComplete(status);

                // CHECK4: Agent Service has permissions to agent trace directory.
                SetStatus(index, Status.InProgress, _totalChecks, ref _passedChecks, ref _failedChecks);
                try
                {
                    resolutionSteps = agentServiceRequest.HasPermissionToTraceDirectory();
                    ;
                    if (string.IsNullOrEmpty(resolutionSteps))
                        status = Status.Passesd;
                    else
                    {
                        status = Status.Failed;
                        resolutionStepsBuilder.AppendFormat("\\b {0} \\b0\\line ", CoreConstants.PermissionCheck4.Replace("\\", "\\\\"));
                        resolutionStepsBuilder.Append(resolutionSteps);
                        resolutionStepsBuilder.Append("\\par");
                    }
                }
                catch (Exception ex)
                {
                    status = Status.Failed;
                    resolutionStepsBuilder.AppendFormat("\\b {0} \\b0\\line ", CoreConstants.PermissionCheck4.Replace("\\", "\\\\"));

                    if (ex is SocketException)
                        resolutionStepsBuilder.Append("A connection could not be made to Agent Server. \\line ");

                    resolutionStepsBuilder.AppendFormat("ERROR: {0} \\line ", ex.Message);
                    resolutionStepsBuilder.Append("\\par");
                    ErrorLog.Instance.Write(ErrorLog.Level.UltraDebug, CoreConstants.PermissionCheck4, ex.Message, ErrorLog.Severity.Informational);
                }
                SetStatus(index++, status, _totalChecks, ref _passedChecks, ref _failedChecks);
                if (OnPermissionCheckComplete != null)
                    OnPermissionCheckComplete(status);

                // CHECK5: Agent Service has rights to read registry HKLM\Software\Idera\SQLCM.
                SetStatus(index, Status.InProgress, _totalChecks, ref _passedChecks, ref _failedChecks);
                try
                {
                    resolutionSteps = agentServiceRequest.HasRightToReadRegistry();
                    if (string.IsNullOrEmpty(resolutionSteps))
                        status = Status.Passesd;
                    else
                    {
                        status = Status.Failed;
                        resolutionStepsBuilder.AppendFormat("\\b {0} \\b0\\line ", CoreConstants.PermissionCheck5.Replace("\\", "\\\\"));
                        resolutionStepsBuilder.Append(resolutionSteps);
                        resolutionStepsBuilder.Append("\\par");
                    }
                }
                catch (Exception ex)
                {
                    status = Status.Failed;
                    resolutionStepsBuilder.AppendFormat("\\b {0} \\b0\\line ", CoreConstants.PermissionCheck5.Replace("\\", "\\\\"));

                    if (ex is SocketException)
                        resolutionStepsBuilder.Append("A connection could not be made to Agent Server. \\line ");

                    resolutionStepsBuilder.AppendFormat("ERROR: {0} \\line ", ex.Message);
                    resolutionStepsBuilder.Append("\\par");
                    ErrorLog.Instance.Write(ErrorLog.Level.UltraDebug, CoreConstants.PermissionCheck5, ex.Message, ErrorLog.Severity.Informational);
                }
                SetStatus(index++, status, _totalChecks, ref _passedChecks, ref _failedChecks);
                if (OnPermissionCheckComplete != null)
                    OnPermissionCheckComplete(status);

                // CHECK6: Agent Service has rights to the instance.
                SetStatus(index, Status.InProgress, _totalChecks, ref _passedChecks, ref _failedChecks);
                try
                {
                    var instance = serverToCheck.Instance;
                    if (serverToCheck.InstancePort != null)
                    {
                        instance = string.Join(",",serverToCheck.Instance, serverToCheck.InstancePort);                        
                    }
                    resolutionSteps = agentServiceRequest.HasRightsToSqlServer(instance);
                    if (string.IsNullOrEmpty(resolutionSteps))
                        status = Status.Passesd;
                    else
                    {
                        status = Status.Failed;
                        resolutionStepsBuilder.AppendFormat("\\b {0} \\b0\\line ", CoreConstants.PermissionCheck6.Replace("\\", "\\\\"));
                        resolutionStepsBuilder.Append(resolutionSteps);
                        resolutionStepsBuilder.Append("\\par");
                    }
                }
                catch (Exception ex)
                {
                    status = Status.Failed;
                    resolutionStepsBuilder.AppendFormat("\\b {0} \\b0\\line ", CoreConstants.PermissionCheck6.Replace("\\", "\\\\"));

                    if (ex is SocketException)
                        resolutionStepsBuilder.Append("A connection could not be made to Agent Server. \\line ");

                    resolutionStepsBuilder.AppendFormat("ERROR: {0} \\line ", ex.Message);
                    resolutionStepsBuilder.Append("\\par");
                    ErrorLog.Instance.Write(ErrorLog.Level.UltraDebug, CoreConstants.PermissionCheck6, ex.Message, ErrorLog.Severity.Informational);
                }
                SetStatus(index++, status, _totalChecks, ref _passedChecks, ref _failedChecks);
                if (OnPermissionCheckComplete != null)
                    OnPermissionCheckComplete(status);

                // CHECK7: SQL Server has permissions to the agent trace directory.
                SetStatus(index, Status.InProgress, _totalChecks, ref _passedChecks, ref _failedChecks);
                try
                {
                    var instance = serverToCheck.Instance;
                    if (serverToCheck.InstancePort != null)
                    {
                        instance = string.Join(",", serverToCheck.Instance, serverToCheck.InstancePort);
                    }
                    resolutionSteps = agentServiceRequest.SqlServerHasPermissionToTraceDirectory(instance);
                    if (string.IsNullOrEmpty(resolutionSteps))
                        status = Status.Passesd;
                    else
                    {
                        status = Status.Failed;
                        resolutionStepsBuilder.AppendFormat("\\b {0} \\b0\\line ", CoreConstants.PermissionCheck7.Replace("\\", "\\\\"));
                        resolutionStepsBuilder.Append("A connection could not be made to Agent Server. \\line ");
                        resolutionStepsBuilder.Append(resolutionSteps);
                        resolutionStepsBuilder.Append("\\par");
                    }
                }
                catch (Exception ex)
                {
                    status = Status.Failed;
                    resolutionStepsBuilder.AppendFormat("\\b {0} \\b0\\line ", CoreConstants.PermissionCheck7.Replace("\\", "\\\\"));

                    if (ex is SocketException)
                        resolutionStepsBuilder.Append("A connection could not be made to Agent Server. \\line ");

                    resolutionStepsBuilder.AppendFormat("ERROR: {0} \\line ", ex.Message);
                    resolutionStepsBuilder.Append("\\par");
                    ErrorLog.Instance.Write(ErrorLog.Level.UltraDebug, CoreConstants.PermissionCheck7, ex.Message, ErrorLog.Severity.Informational);
                }
                SetStatus(index++, status, _totalChecks, ref _passedChecks, ref _failedChecks);
                if (OnPermissionCheckComplete != null)
                    OnPermissionCheckComplete(status);

                // CHECK8: SQL Server has permissions to the collection trace directory.
                SetStatus(index, Status.InProgress, _totalChecks, ref _passedChecks, ref _failedChecks);
                try
                {
                    resolutionSteps =
                        collectionServiceRequest.SqlServerHasPermissionToTraceDirectory(Globals.Repository.Instance);
                    if (string.IsNullOrEmpty(resolutionSteps))
                        status = Status.Passesd;
                    else
                    {
                        status = Status.Failed;
                        resolutionStepsBuilder.AppendFormat("\\b {0} \\b0\\line ", CoreConstants.PermissionCheck8.Replace("\\", "\\\\"));
                        resolutionStepsBuilder.Append(resolutionSteps);
                        resolutionStepsBuilder.Append("\\par");
                    }
                }
                catch (Exception ex)
                {
                    status = Status.Failed;
                    resolutionStepsBuilder.AppendFormat("\\b {0} \\b0\\line ", CoreConstants.PermissionCheck8.Replace("\\", "\\\\"));

                    if (ex is SocketException)
                        resolutionStepsBuilder.Append("A connection could not be made to Collection Server. \\line ");

                    resolutionStepsBuilder.AppendFormat("ERROR: {0} \\line ", ex.Message);
                    resolutionStepsBuilder.Append("\\par");
                    ErrorLog.Instance.Write(ErrorLog.Level.UltraDebug, CoreConstants.PermissionCheck8, ex.Message, ErrorLog.Severity.Informational);
                }
                SetStatus(index++, status, _totalChecks, ref _passedChecks, ref _failedChecks);
                if (OnPermissionCheckComplete != null)
                    OnPermissionCheckComplete(status);

                _checkStatus = _failedChecks > 0 || _passedChecks != _totalChecks ? Status.Failed : Status.Passesd;

                if (_checkStatus == Status.Failed)
                {
                    // disable logging to event logs
                    if (!eventLoggingEnabled)
                        ErrorLog.Instance.LogToEventLog = false;

                    _resolutionStepsRtf = resolutionStepsBuilder.ToString();

                    // show resolution steps
                    if (IsResolutionStepsShownOnFailure)
                        ShowResolutionSteps(_failedChecks, _resolutionStepsRtf, IsUserActivityBlocked);


                }
            }
            catch (ThreadAbortException)
            {
                _checkStatus = Status.Cancelled;
            }
            finally
            {
                // enable user input
                btnReCheck.Enabled = true;
                Cursor = Cursors.Default;

                if (OnPermissionsCheckCompleted != null)
                    OnPermissionsCheckCompleted(_checkStatus);
            }
        }

        #endregion

        public void Reset()
        {
            _passedChecks = 0;
            _failedChecks = 0;
            _checkStatus = Status.Waiting;
            _blockUserActivity = false;
            _resolutionStepsRtf = string.Empty;
            btnReCheck.Enabled = IsReCheckAllowed;

            // reset all checks to waiting
            for (var index = 0; index < lstChecks.Items.Count; index += 1)
                SetStatus(index, Status.Waiting, _totalChecks, ref _passedChecks, ref _failedChecks);
        }

        public void StopPermissionsCheck()
        {
            if (_checkStatus != Status.InProgress)
                return;

            if (_permissionsChecker != null)
            {
                _permissionsChecker.Abort();
                _permissionsChecker = null;
            }
        }

        public void StartPermissionsCheck(ServerRecord server)
        {
            if (_checkStatus == Status.InProgress || server == null)
                return;

            _server = server;

            _permissionsChecker = new Thread(PermissionsChecker);
            _permissionsChecker.IsBackground = true;
            _permissionsChecker.Priority = ThreadPriority.Normal;
            _permissionsChecker.Start(server);
        }

        public void ShowResolutionSteps(int failedChecks, string resolutionStepsRtf, bool blockUserActivity)
        {
            if (string.IsNullOrEmpty(resolutionStepsRtf) || resolutionStepsRtf.Trim().Length == 0)
                return;

            var permissionsCheckFailed = new Form_AlertPermissionsCheckFailed(failedChecks, resolutionStepsRtf, IsReCheckAllowed);
            _blockUserActivity = permissionsCheckFailed.ShowDialog(ParentForm) == DialogResult.OK;
        }

        private void btnReCheck_Click(object sender, EventArgs e)
        {
            StartPermissionsCheck(_server);
        }

        private void lnkHelp_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                HelpAlias.ShowHelp(this, HelpAlias.SSHELP_PermissionsCheck);
                lnkHelp.LinkVisited = true;
            }
        }

        private void PermissionsCheck_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            HelpAlias.ShowHelp(this, HelpAlias.SSHELP_PermissionsCheck);
        }
    }
}
