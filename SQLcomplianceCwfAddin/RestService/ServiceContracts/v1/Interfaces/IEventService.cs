using System.Collections.Generic;
using System.ComponentModel;
using System.ServiceModel;
using System.ServiceModel.Web;
using SQLcomplianceCwfAddin.RestService.DataContracts.v1;
using SQLcomplianceCwfAddin.RestService.DataContracts.v1.Stats;
using SQLcomplianceCwfAddin.RestService.DataContracts.v1.Events;

namespace SQLcomplianceCwfAddin.RestService.ServiceContracts.v1.Interfaces
{
    [ServiceContract]
    public interface IEventService
    {
        [OperationContract(AsyncPattern = false)]
        [WebInvoke(Method = "POST", UriTemplate = "/GetEventProperties", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Get SQL Compliance Manager agent alert event properties.")]
        EventProperties GetEventProperties(EventPropertiesRequest request);

        [OperationContract(AsyncPattern = false)]
        [WebInvoke(Method = "GET", UriTemplate = "/GetRecentDatabaseEvents?instanceId={serverId}&databaseId={databaseId}&days={days}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Get recent activity for database in instance.")]
        IEnumerable<EventProperties> GetRecentDatabaseEvents(int serverId, int databaseId, int days);

        [OperationContract(AsyncPattern = false)]
        [WebInvoke(Method = "GET", UriTemplate = "/GetEvents?instanceId={serverId}&days={days}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Get data for recent audit events widget in indivisual instance view.")]
        IEnumerable<AuditEvent> GetEvents(int serverId, int days);

        [OperationContract(AsyncPattern = false)]
        [WebInvoke(Method = "POST", UriTemplate = "/GetAuditedEvents", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Get data for recent audit events widget in indivisual instance view.")]
        DetaliedEventsResponse GetAuditedEvents(FilteredEventRequest request);

        [OperationContract(AsyncPattern = false)]
        [WebInvoke(Method = "GET", UriTemplate = "/GetEventsByCategoryAndDateForInstance?instanceId={serverId}&days={days}&category={category}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Get data for recent audit events widget in indivisual instance view.")]
        DetaliedEventsResponse GetEventsByCategoryAndDateForInstance(int serverId, int days, RestStatsCategory category);

        [OperationContract(AsyncPattern = false)]
        [WebInvoke(Method = "GET", UriTemplate = "/GetEventsByIntervalForInstance?instanceId={serverId}&days={days}&page={page}&pageSize={pageSize}&sortColumn={sortColumn}&sortDirection={sortDirection}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Get data for recent audit events widget in indivisual instance view.")]
        DetaliedEventsResponse GetEventsByIntervalForInstance(int serverId, int days, int page, int pageSize, string sortColumn, int sortDirection);

        [OperationContract(AsyncPattern = false)]
        [WebInvoke(Method = "GET", UriTemplate = "/GetEventsByIntervalForDatabase?serverId={serverId}&databaseId={databaseId}&days={days}&page={page}&pageSize={pageSize}&sortColumn={sortColumn}&sortDirection={sortDirection}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Get data for recent audit events widget in individual database view.")]
        DetaliedEventsResponse GetEventsByIntervalForDatabase(int serverId, int databaseId, int days, int page, int pageSize, string sortColumn, int sortDirection);

        [OperationContract(AsyncPattern = false)]
        [WebInvoke(Method = "POST", UriTemplate = "/GetArchivedEvents", RequestFormat = WebMessageFormat.Json,ResponseFormat = WebMessageFormat.Json)]
        [Description("Get data for recent audit events widget in indivisual instance view.")]
        DetaliedEventsResponse GetArchivedEvents(FilteredEventRequest request);

        [OperationContract(AsyncPattern = false)]
        [WebInvoke(Method = "GET", UriTemplate = "/GetEventDistributionForDatabase?instanceId={serverId}&databaseId={databaseId}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Get event distribution for database in instance.")]
        EventDistributionForDatabaseResult GetEventDistributionForDatabase(int serverId, int databaseId);

        [OperationContract(AsyncPattern = false)]
        [WebInvoke(Method = "GET", UriTemplate = "/GetDatabaseEvents?instanceId={serverId}&databaseId={databaseId}&days={days}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Get data for recent database audit events widget in indivisual database view.")]
        IEnumerable<AuditEvent> GetDatabaseEvents(int serverId, int databaseId, int days);

        [OperationContract(AsyncPattern = false)]
        [WebInvoke(Method = "POST", UriTemplate = "/GetAuditedEventFilter", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Get alerts based on filter.")]
        AuditEventFilterResponse GetAuditEventFilter(AuditEventFilterRequest request);

        [OperationContract(AsyncPattern = false)]
        [WebInvoke(Method = "POST", UriTemplate = "/GetEnableAuditEventFilter", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Enable or disable Event Filter")]
        void GetEnableAuditEventFilter(EnableAuditEventFilter request);

        [OperationContract(AsyncPattern = false)]
        [WebInvoke(Method = "POST", UriTemplate = "/GetAuditEventFilterDelete", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Delete Audit  Event Filter")]
        void GetAuditEventFilterDelete(EnableAuditEventFilter request);

        [OperationContract(AsyncPattern = false)]
        [WebInvoke(Method = "POST", UriTemplate = "/GetAuditEventFilterExport", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Get Audit Event Filter Data to Export")]
        AuditEventExportResponse GetAuditEventFilterExport(AuditEventExportRequest request);

        [OperationContract(AsyncPattern = false)]
        [WebInvoke(Method = "POST", UriTemplate = "/InsertEventFilterData", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Enable or disable alert Rules.")]
        void InsertStatusEventFilter(InsertStatusEventFilterRequest request);

        [OperationContract(AsyncPattern = false)]
        [WebInvoke(Method = "POST", UriTemplate = "/ImportAuditEventFilters", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Import Event Filter.")]
        string ImportAuditEventFilters(string request);

        [OperationContract(AsyncPattern = false)]
        [WebInvoke(Method = "POST", UriTemplate = "/ExportEventFilter", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Export Alert Rules by Id")]
        string ExportEventFilter(string request);

    }
}
