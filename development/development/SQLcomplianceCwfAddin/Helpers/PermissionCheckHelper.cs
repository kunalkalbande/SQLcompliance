using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using Idera.SQLcompliance.Core;
using SQLcomplianceCwfAddin.Helpers.SQL;
using SQLcomplianceCwfAddin.RestService.DataContracts.v1.AddDatabase;
using CollectorManagementConsoleRequest = Idera.SQLcompliance.Core.Collector.ManagementConsoleRequest;
using AgentManagementConsoleRequest = Idera.SQLcompliance.Core.Agent.ManagementConsoleRequest;

namespace SQLcomplianceCwfAddin.Helpers
{
    public class PermissionCheckHelper : Singleton<PermissionCheckHelper>
    {
        private static readonly int DEFAULT_TASK_RUNNING_TIMEOUT_VALUE = 50;//in seconds
        private static readonly double DEFAULT_CANCELATION_TASK_TIMEOUT_VALUE = 0.5;//in seconds

        #region private members

        #endregion

        #region private methods

        private Dictionary<string, Func<string>> CreatePermissionCheckFunctionDictionary(CollectorManagementConsoleRequest collectorRequest, AgentManagementConsoleRequest agentRequest, string serverInstance, string collectionInstance)
        {
            var permissionCheckFunctionDictionary = new Dictionary<string, Func<string>>();
            permissionCheckFunctionDictionary.Add(CoreConstants.PermissionCheck1, collectorRequest.HasRoghtsToRepository);
            permissionCheckFunctionDictionary.Add(CoreConstants.PermissionCheck2, collectorRequest.HasRightToReadRegistry);
            permissionCheckFunctionDictionary.Add(CoreConstants.PermissionCheck3, collectorRequest.HasPermissionToTraceDirectory);
            permissionCheckFunctionDictionary.Add(CoreConstants.PermissionCheck4, agentRequest.HasPermissionToTraceDirectory);
            permissionCheckFunctionDictionary.Add(CoreConstants.PermissionCheck5, agentRequest.HasRightToReadRegistry);
            permissionCheckFunctionDictionary.Add(CoreConstants.PermissionCheck6, () => agentRequest.HasRightsToSqlServer(serverInstance));
            permissionCheckFunctionDictionary.Add(CoreConstants.PermissionCheck7, () => agentRequest.SqlServerHasPermissionToTraceDirectory(serverInstance));
            permissionCheckFunctionDictionary.Add(CoreConstants.PermissionCheck8, () => collectorRequest.SqlServerHasPermissionToTraceDirectory(collectionInstance));

            return permissionCheckFunctionDictionary;
        }

        private bool RunPermissionCheckTaskWithTimeOut(Task<PermissionCheck> permissionCheckTask, TimeSpan timeout)
        {
            bool isTimeout = true;

            permissionCheckTask.Start();
            var index = Task.WaitAny(new[] { permissionCheckTask }, timeout);

            if (index == -1)
            {
                var cts = new CancellationTokenSource();
                var cancellable = permissionCheckTask.ContinueWith(ignored => { }, cts.Token);
                cts.Cancel();
                Task.WaitAny(new[] { cancellable }, TimeSpan.FromSeconds(DEFAULT_CANCELATION_TASK_TIMEOUT_VALUE));
            }
            else
            {
                isTimeout = false;
            }

            return isTimeout;
        }

        private PermissionCheck VerifyPermissionCheck(string checkName, Func<string> getResolutionStepsFunction )
        {
            var check = new PermissionCheck();
            check.Name = checkName;
            check.Status = CheckStatus.Failed;

            try
            {
                var resolutionSteps = getResolutionStepsFunction();
                if (string.IsNullOrEmpty(resolutionSteps))
                {
                    check.Status = CheckStatus.Passesd;
                }
                else
                {
                    check.ResolutionSteps = resolutionSteps;
                }
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.UltraDebug, checkName, ex.Message, ErrorLog.Severity.Informational);
                check.ResolutionSteps = ex.Message;
            }

            return check;
        }

        private List<PermissionCheck> RunPermissionChecksForAgentAndCollecionService(CollectorManagementConsoleRequest collectorRequest, AgentManagementConsoleRequest agentRequest, string serverInstance,string collectionInstance)
        {

            var checkList = new List<PermissionCheck>();
            var permissionCheckFunctionDictionary = CreatePermissionCheckFunctionDictionary(collectorRequest, agentRequest, serverInstance, collectionInstance);

            Parallel.ForEach(permissionCheckFunctionDictionary.Keys, permissionCheck =>
            {
                var permissionTask =
                    new Task<PermissionCheck>(
                        () =>
                            VerifyPermissionCheck(CoreConstants.PermissionCheck1,
                                () => permissionCheckFunctionDictionary[permissionCheck]()));

                var check = new PermissionCheck
                {
                    Name = permissionCheck,
                    Status = CheckStatus.Failed,
                };

                if (RunPermissionCheckTaskWithTimeOut(permissionTask,
                    TimeSpan.FromSeconds(DEFAULT_TASK_RUNNING_TIMEOUT_VALUE)))
                {
                    check.ResolutionSteps =
                        "Permission check was timeout. Please check security access setttings for this permission check.";
                }
                else
                {
                    check.Status = permissionTask.Result.Status;
                    check.ResolutionSteps = permissionTask.Result.ResolutionSteps;
                }

                checkList.Add(check);
            });

            return checkList;
        }

        private void UpdateStatistic(PermissionChecksStatus status)
        {
            status.TotalChecks = status.PermissionsCheckList.Count;
            status.PassedChecks = 0;
            status.FailedChecks = 0;
            foreach (var check in status.PermissionsCheckList)
            {
                if (check.Status == CheckStatus.Passesd)
                {
                    status.PassedChecks++;
                }
                else
                {
                    status.FailedChecks++;
                }
            }
        } 

        #endregion

        #region public methods
        public PermissionChecksStatus VerifyPermissions(int serverId, SqlConnection connection)
        {
            try
            {
                var checksStatus = new PermissionChecksStatus();
                var config = SqlCmConfigurationHelper.GetConfiguration(connection);

                if (config == null)
                {
                    throw new Exception(string.Format("Failed to get SQL CM configuration for connection: {0}", connection));
                }

                var collectorServiceRequest = ProxyObjecHelper.CreateProxyObject<CollectorManagementConsoleRequest>(config.Server, config.ServerPort);
                var server = SqlCmRecordReader.GetServerRecord(serverId, connection);
                var agentServiceRequest = ProxyObjecHelper.CreateProxyObject<AgentManagementConsoleRequest>(server.AgentServer, server.AgentPort);
                var checks = RunPermissionChecksForAgentAndCollecionService(collectorServiceRequest, agentServiceRequest, server.Instance, config._repositoryServer);
                checksStatus.PermissionsCheckList = new List<PermissionCheck>(checks);
                UpdateStatistic(checksStatus);
                return checksStatus;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Failed to Verify Permissions due to the following error: {0}", ex));
            }

        } 
        #endregion
    }
}
