using System.Collections.Generic;
using SQLcomplianceCwfAddin.Helpers;
using SQLcomplianceCwfAddin.RestService.DataContracts.v1;
using SQLcomplianceCwfAddin.RestService.DataContracts.v1.Alerts;

namespace SQLcomplianceCwfAddin.RestService.ServiceContracts.v1
{
    public partial class RestService
    {
        public IEnumerable<ServerAlert> GetAuditedInstancesAlert(AlertType alertType)
        {
            using (_logger.InfoCall("GetAuditedInstancesAlert"))
            {
                var query = QueryBuilder.Instance.GetInstancesAlerts(alertType);
                var result = QueryExecutor.Instance.GetAlertsData(GetConnection(), query);
                return result;
            }
        }

        public void DismissAlert(int alertId)
        {
            using (_logger.InfoCall("DismissAlert"))
            {
                string query = QueryBuilder.Instance.DismissAlert(alertId);
                QueryExecutor.Instance.ExecuteNonQuery(GetConnection(), query);
            }
        }

        public void DismissAlertList(IdCollection idCollection)
        {
            using (_logger.InfoCall("DismissAlertList"))
            {
                string query = QueryBuilder.Instance.DismissAlertList(idCollection);
                QueryExecutor.Instance.ExecuteNonQuery(GetConnection(), query);
            }
        }

        public void DismissAlertsGroupForInstance(DismissAlertsGroupRequest request)
        {
            using (_logger.InfoCall("DismissAlertsGroupForInstance"))
            {
                QueryExecutor.Instance.DismissAlertsGroupForInstance(GetConnection(), request);
            }
        }

        public FilteredAlertsResponse GetFilteredAlerts(FilteredAlertRequest request)
        {
            using (_logger.InfoCall("GetFilteredAlerts"))
            {
                var result = QueryExecutor.Instance.GetFilteredAlerts(GetConnection(), request);
                return result;
            }
        }

        public IEnumerable<ServerAlert> GetAlertsForInstance(int serverId)
        {
            using (_logger.InfoCall("GetInstancesAlerts"))
            {
                var query = QueryBuilder.Instance.GetAlertsForInstance(serverId);
                var result = QueryExecutor.Instance.GetAlertsData(GetConnection(), query);

                return result;
            }
        }

        public IEnumerable<ServerAlert> GetAlertsByDatabases(string days)
        {
            using (_logger.InfoCall("GetAlertsByDatabases"))
            {
                var query = QueryBuilder.Instance.GetAlertsByDatabases();
                var result = QueryExecutor.Instance.GetAlertsByDatabases(GetConnection(), query, days);

                return result;
            }
        }

        public IEnumerable<AlertsGroup> GetAlertsGroups(string instanceId)
        {
            using (_logger.InfoCall("GetAlertsGroups"))
            {
                int instanceIdInt;
                int.TryParse(instanceId, out instanceIdInt);

                var query = QueryBuilder.Instance.GetAlertsGroups();
                var result = QueryExecutor.Instance.GetAlertsGroups(GetConnection(), query, instanceIdInt);

                return result;
            }
        }

        public IEnumerable<ServerAlert> GetAlerts(string instanceId, AlertType alertType, AlertLevel alertLevel, int pageSize, int page)
        {
            using (_logger.InfoCall("GetAlertsGroups"))
            {
                int instanceIdInt;
                int.TryParse(instanceId, out instanceIdInt);

                var query = QueryBuilder.Instance.GetAlerts();
                var result = QueryExecutor.Instance.GetAlerts(GetConnection(), query, instanceIdInt, alertType, alertLevel, pageSize, page);

                return result;
            }
        }
    }
}
