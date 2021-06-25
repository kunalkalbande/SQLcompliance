using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using SQLcomplianceCwfAddin.Helpers;
using SQLcomplianceCwfAddin.Helpers.SQL;
using SQLcomplianceCwfAddin.RestService.DataContracts.v1;
using SQLcomplianceCwfAddin.RestService.DataContracts.v1.Stats;
using Idera.SQLcompliance.Core.Agent;
using Idera.SQLcompliance.Core.Stats;
using SQLcomplianceCwfAddin.Helpers.Stats;

namespace SQLcomplianceCwfAddin.RestService.ServiceContracts.v1
{
    public partial class RestService
    {
        public List<KeyValuePair<RestStatsCategory, List<RestStatsData>>> GetDatabaseStatsData(int serverId, int databaseId, int days, RestStatsCategory category)
        {
            using (_logger.InfoCall("GetDatabaseStatsData"))
            {
                using (var connection = GetConnection())
                {
                    var servers = new List<ServerRecord>();
                    var serverRecord = SqlCmRecordReader.GetServerRecord(serverId, connection);
                    servers.Add(serverRecord);
                    return StatsAggregator.GetStatsDataInternal(servers, databaseId, days, new List<RestStatsCategory> { category }, connection);
                }
            }
        }

        public EnvironmentDetailsForInstancesAndDatabases GetEnvironmentDetailsForInstancesAndDatabases(string days)
        {
            using (_logger.InfoCall("GetEnvironmentDetailsForInstancesAndDatabases"))
            {
                EnvironmentDetailsForInstancesAndDatabases result = new EnvironmentDetailsForInstancesAndDatabases();

                var alertStatus = GetEnvironmentAlertStatus(days);
                var envDetails = GetEnvironmentDetails(days);

                result.AuditedDatabaseCount = envDetails.AuditedDatabaseCount;
                result.AuditedInstanceCount = envDetails.AuditedSqlServerCount;
                result.AlertStatus = alertStatus;

                return result;
            }
        }

        public EnvironmentAlertStatus GetEnvironmentAlertStatus(string days)
        {
            using (_logger.InfoCall("GetEnvironmentAlertStatus"))
            {
                var request = new FilteredRegisteredInstancesStatusRequest();
                int iDays;
                if (int.TryParse(days, out iDays))
                    request.Days = iDays;

                var data = GetFilteredAuditedInstancesStatus(request);
                EnvironmentAlertStatus alertStatus = new EnvironmentAlertStatus();
                foreach (var auditedServerStatus in data)
                {
                    alertStatus.AuditedInstances.Low += auditedServerStatus.LowAlerts;
                    alertStatus.AuditedInstances.Medium += auditedServerStatus.MediumAlerts;
                    alertStatus.AuditedInstances.High += auditedServerStatus.HighAlerts;
                    alertStatus.AuditedInstances.Severe += auditedServerStatus.SevereAlerts;
                }

                alertStatus.AuditedInstances.Total = alertStatus.AuditedInstances.Low +
                                                     alertStatus.AuditedInstances.Medium +
                                                     alertStatus.AuditedInstances.High +
                                                     alertStatus.AuditedInstances.Severe;

                IEnumerable<ServerAlert> databaseAlerts = GetAlertsByDatabases(days);
                foreach (var databaseAlert in databaseAlerts)
                {
                    switch (databaseAlert.Level)
                    {
                        case AlertLevel.Low:
                            alertStatus.AuditedDatabases.Low++;
                            break;
                        case AlertLevel.Medium:
                            alertStatus.AuditedDatabases.Medium++;
                            break;
                        case AlertLevel.High:
                            alertStatus.AuditedDatabases.High++;
                            break;
                        case AlertLevel.Severe:
                            alertStatus.AuditedDatabases.Severe++;
                            break;
                    }
                }

                alertStatus.AuditedDatabases.Total = alertStatus.AuditedDatabases.Low +
                                                     alertStatus.AuditedDatabases.Medium +
                                                     alertStatus.AuditedDatabases.High
                                                     + alertStatus.AuditedDatabases.Severe;

                return alertStatus;
            }
        }

        public IEnumerable<EnvironmentObject> GetEnvironmentObjects(string objectId, EnvironmentObjectType type)
        {
            using (_logger.InfoCall("GetEnvironmentObjects"))
            {
                var query = QueryBuilder.Instance.GetEnvironmentObjectHierarchy(objectId, type);

                if (query == null)
                    return new List<EnvironmentObject>(0);

                // return -1 parent ID if parsing fails
                int parentId;
                if (!int.TryParse(objectId, out parentId))
                    parentId = -1;

                var result = QueryExecutor.Instance.GetEnvironmentObjectHierarchy(GetConnection(), query, parentId);
                return result;
            }
        }

        public EnvironmentDetails GetEnvironmentDetails(string days)
        {
            using (_logger.InfoCall("GetEnvironmentDetails"))
            {
                var request = new FilteredRegisteredInstancesStatusRequest();
                int iDays;
                if (int.TryParse(days, out iDays))
                    request.Days = iDays;

                var data = GetFilteredAuditedInstancesStatus(request);

                EnvironmentDetails details = new EnvironmentDetails();
                bool errorsFound = false, warningsFound = false;

                foreach (var auditedServerStatus in data)
                {
                    if (auditedServerStatus.IsAudited)
                    {
                        details.RegisteredSqlServerCount++;
                        if (auditedServerStatus.IsEnabled)
                        {
                            details.AuditedSqlServerCount++;
                            details.AuditedDatabaseCount += auditedServerStatus.AuditedDatabaseCount;
                        }
                    }

                    details.ProcessedEventCount += auditedServerStatus.CollectedEventCount;
                    if (auditedServerStatus.ServerStatusDetailed == ServerStatus.Warning)
                        warningsFound = true;
                    if (auditedServerStatus.ServerStatusDetailed == ServerStatus.Alert)
                        errorsFound = true;
                }

                if (details.RegisteredSqlServerCount == 0)
                    details.EnvironmentHealth = EnvironmentHealth.NoInstances;
                else if (errorsFound)
                    details.EnvironmentHealth = EnvironmentHealth.Error;
                else if (warningsFound)
                    details.EnvironmentHealth = EnvironmentHealth.Warning;

                return details;
            }
        }

        public List<KeyValuePair<RestStatsCategory, List<RestStatsData>>> GetStatsData(string serverId, int days, RestStatsCategory category)
        {
            using (_logger.InfoCall("GetStatsData"))
            {
                int? srvId = null;
                int tempSrvId;
                if (int.TryParse(serverId, out tempSrvId))
                    srvId = tempSrvId;

                using( var connection = GetConnection() )
        {

                    var servers = new List<ServerRecord>();

                    if (srvId.HasValue)
                    {
                        var serverRecord = SqlCmRecordReader.GetServerRecord(srvId.Value, connection);
                        servers.Add(serverRecord);
                    }
                    else
                    {
                        servers = ServerRecord.GetServers(connection, true);
                    }

                    if (category.ToString() == "WidgetValue")
                    {
                        return StatsAggregator.GetStatsDataInternal(servers, null, days, new List<RestStatsCategory> { RestStatsCategory.Alerts, RestStatsCategory.Security, RestStatsCategory.PrivUserEvents, RestStatsCategory.FailedLogin, RestStatsCategory.Ddl }, connection);
                    }
                    return StatsAggregator.GetStatsDataInternal(servers, null, days, new List<RestStatsCategory> { category }, connection);
                }
            }
        }
    }
}
