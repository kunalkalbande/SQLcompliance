using System;
using System.Collections.Generic;
using SQLcomplianceCwfAddin.Helpers;
using SQLcomplianceCwfAddin.RestService.DataContracts.v1;
using SQLcomplianceCwfAddin.RestService.DataContracts.v1.Events;
using SQLcomplianceCwfAddin.RestService.DataContracts.v1.Stats;
using Idera.SQLcompliance.Core.Event;
using SQLcomplianceCwfAddin.Helpers.Events;

namespace SQLcomplianceCwfAddin.RestService.ServiceContracts.v1
{
    public partial class RestService
    {
        public EventProperties GetEventProperties(EventPropertiesRequest request)
        {
            using (_logger.InfoCall("GetEventProperties"))
            {
                using (var connection = GetConnection())
                {
                    return EventsManagerHelper.Instance.GetEventProperties(request, connection);
                }
            }
        }

        public IEnumerable<EventProperties> GetRecentDatabaseEvents(int serverId, int databaseId, int days)
        {
            using (_logger.InfoCall("GetRecentDatabaseEvents"))
            {
                string eventDatabase = QueryExecutor.Instance.GetEventDatabaseName(GetConnection().ConnectionString, serverId);

                string query = QueryBuilder.Instance.GetRecentDatabaseEvents(eventDatabase, days, databaseId);
                var result = QueryExecutor.Instance.GetRecentDatabaseEvents(GetConnection(), query);

                return result;
            }
        }

        public IEnumerable<AuditEvent> GetEvents(int serverId, int days)
        {
            using (_logger.InfoCall("GetEvents"))
            {
                var eventDatabase = QueryExecutor.Instance.GetEventDatabaseName(GetConnection().ConnectionString, serverId);
                var query = QueryBuilder.Instance.GetEvents(eventDatabase, days);
                var result = QueryExecutor.Instance.GetDataForRecentAuditEventsWidget(GetConnection(), query);
                return result;
            }
        }


        private string GetEventDatabase(int serverId)
            {
            string eventDatabase = string.Empty;

                try
                {
                eventDatabase = QueryExecutor.Instance.GetEventDatabaseName(GetConnection().ConnectionString, serverId);
                }
                catch (Exception e)
                {
                    _logger.Error(e.Message);
                    throw new Exception("Couldn't get event database for given instance.");
                }

            return eventDatabase;
        }

        public DetaliedEventsResponse GetAuditedEvents(FilteredEventRequest request)
        {
            using (_logger.InfoCall("GetAuditedEvents"))
            {
                var eventDatabase = GetEventDatabase(request.ServerId);

                var query = QueryBuilder.Instance.GetAuditedEvents();
                try
                {
                    return QueryExecutor.Instance.GetAuditedEvents(GetConnection(), query, eventDatabase, request);
                }
                catch (Exception e)
                {
                    _logger.Error(e.Message);
                    throw new Exception("Couldn't get audited events for given instance.");
                }
            }
        }

        public DetaliedEventsResponse GetEventsByIntervalForDatabase(int serverId, int databaseId, int days, int page, int pageSize, string sortColumn, int sortDirection)
        {
            using (_logger.InfoCall("GetEventsByIntervalForDatabase"))
            {
                FilteredEventRequest filteredEventRequest = new FilteredEventRequest();
                filteredEventRequest.ServerId = serverId;
                filteredEventRequest.DatabaseId = databaseId;
                filteredEventRequest.Page = page;
                filteredEventRequest.PageSize = pageSize;
                filteredEventRequest.SortColumn = sortColumn;
                filteredEventRequest.SortDirection = sortDirection;

                DateTime endDate = DateTime.Now;
                DateTime startDate = endDate.AddDays(-days);

                filteredEventRequest.DateFrom = startDate;
                filteredEventRequest.DateTo = endDate;

                return GetAuditedEvents(filteredEventRequest);
            }
        }

        public DetaliedEventsResponse GetEventsByIntervalForInstance(int serverId, int days, int page, int pageSize, string sortColumn, int sortDirection)
        {
            using (_logger.InfoCall("GetEventsByIntervalForInstance"))
            {
                FilteredEventRequest filteredEventRequest = new FilteredEventRequest();
                filteredEventRequest.ServerId = serverId;
                filteredEventRequest.Page = page;
                filteredEventRequest.PageSize = pageSize;
                filteredEventRequest.SortColumn = sortColumn;
                filteredEventRequest.SortDirection = sortDirection;

                DateTime endDate = DateTime.Now;
                DateTime startDate = endDate.AddDays(-days);

                filteredEventRequest.DateFrom = startDate;
                filteredEventRequest.DateTo = endDate;

                return GetAuditedEvents(filteredEventRequest);
            }
        }

        public DetaliedEventsResponse GetEventsByCategoryAndDateForInstance(int serverId, int days, RestStatsCategory category)
        {
            using (_logger.InfoCall("GetEventsByCategoryAndDateForInstance"))
            {
                FilteredEventRequest filteredEventRequest = new FilteredEventRequest();
                filteredEventRequest.ServerId = serverId;
                DateTime endDate = DateTime.Now;
                DateTime startDate = endDate.AddDays(-days);

                filteredEventRequest.DateFrom = startDate;
                filteredEventRequest.DateTo = endDate;

                if (category != RestStatsCategory.Unknown)
                    filteredEventRequest.StatCategory = category;

                return GetAuditedEvents(filteredEventRequest);
            }
        }

        public DetaliedEventsResponse GetArchivedEvents(FilteredEventRequest request)
        {
            using (_logger.InfoCall("GetArchivedEvents"))
            {
                var query = QueryBuilder.Instance.GetArchivedEvents();
                var eventDatabase = GetEventDatabase(request.ServerId);
                var result = QueryExecutor.Instance.GetArchivedEvents(GetConnection(), query, eventDatabase, request);
                return result;
            }
        }

        public EventDistributionForDatabaseResult GetEventDistributionForDatabase(int serverId, int databaseId)
        {
            using (_logger.InfoCall("GetEventDistributionForDatabase"))
            {
                var eventDatabase = QueryExecutor.Instance.GetEventDatabaseName(GetConnection().ConnectionString, serverId);
                var query = QueryBuilder.Instance.GetEventDistributionForDatabase(eventDatabase, 1, databaseId);
                var result = QueryExecutor.Instance.GetEventDistributionForDatabase(GetConnection(), query);
                return result;
            }
        }

        public IEnumerable<AuditEvent> GetDatabaseEvents(int serverId, int databaseId, int days)
        {
            using (_logger.InfoCall("GetDatabaseEvents"))
            {
                var eventDatabse = QueryExecutor.Instance.GetEventDatabaseName(GetConnection().ConnectionString, serverId);
                var query = QueryBuilder.Instance.GetDatabaseEvents(eventDatabse, days, databaseId);
                var result = QueryExecutor.Instance.GetDataForRecentAuditEventsWidget(GetConnection(), query);
                return result;
            }
        }

        public AuditEventFilterResponse GetAuditEventFilter(AuditEventFilterRequest request)
        {
            Console.Out.WriteLine("inside Event Filter");
            using (_logger.InfoCall("GetAuditEventFilter"))
            {
                var query = QueryBuilder.Instance.GetAuditEventFilter();
                var result = QueryExecutor.Instance.GetAuditEventFilter(GetConnection(), query, request);

                return result;
            }
        }

        public void GetEnableAuditEventFilter(EnableAuditEventFilter request)
        {
            using (_logger.InfoCall("GetEnableAuditEventFilter"))
            {
                using (var connection = GetConnection())
                {
                    var query = QueryBuilder.Instance.GetEnableAuditEventFilter();
                    QueryExecutor.Instance.GetEnableAuditEventFilter(GetConnection(), query, request);
                }
            }
        }

        public void GetAuditEventFilterDelete(EnableAuditEventFilter request)
        {
            using (_logger.InfoCall("GetAuditEventFilterDelete"))
            {
                using (var connection = GetConnection())
                {
                    var query = QueryBuilder.Instance.GetAuditEventFilterDelete();
                    QueryExecutor.Instance.GetAuditEventFilterDelete(GetConnection(), query, request);
                }
            }
        }

        public AuditEventExportResponse GetAuditEventFilterExport(AuditEventExportRequest request)
        {
            using (_logger.InfoCall("GetAuditEventFilterExport"))
            {
                using (var connection = GetConnection())
                {
                    var query = QueryBuilder.Instance.GetDataByFilterId();
                    var result = QueryExecutor.Instance.GetAuditEventExportList(GetConnection(), query, request);
                    return result;
                }
            }
        }

        public void InsertStatusEventFilter(InsertStatusEventFilterRequest request)
        {
            using (_logger.InfoCall("GetInsertAuditEventFilter"))
            {
                using (var connection = GetConnection())
                {
                    var query = QueryBuilder.Instance.InsertStatusEventFilter();
                    QueryExecutor.Instance.InsertStatusEventFilter(GetConnection(), query, request);
                }
            }
        }

        public string ImportAuditEventFilters(string request)
        {
            using (_logger.InfoCall("Import Audit Event Filters"))
            {
                using (var connection = GetConnection())
                {
                    var result = QueryExecutor.Instance.ImportAuditEventFilters(GetConnection(), request);
                    return result;
                }
            }
        }

        public string ExportEventFilter(string request)
        {
            using (_logger.InfoCall("Export Audit Event Filters"))
            {
                var result = QueryExecutor.Instance.ImportCwfEventFilter(GetConnection(), request);
                return result;
            }
        }
    }
}
