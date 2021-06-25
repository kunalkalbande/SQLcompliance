using System.Collections.Generic;
using System.ComponentModel;
using System.ServiceModel;
using System.ServiceModel.Web;
using SQLcomplianceCwfAddin.RestService.DataContracts.v1;
using SQLcomplianceCwfAddin.RestService.DataContracts.v1.Archives;

namespace SQLcomplianceCwfAddin.RestService.ServiceContracts.v1.Interfaces
{
    [ServiceContract]
    public interface IArchivesService
    {
        [OperationContract(AsyncPattern = false)]
        [WebInvoke(Method = "POST", UriTemplate = "/GetArchivesList", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Get archives list.")]
        IEnumerable<ArchiveRecord> GetArchivesList(ArchiveInstanceRequest archveInstanceRequest);

        [OperationContract(AsyncPattern = false)]
        [WebInvoke(Method = "POST", UriTemplate = "/GetArchiveProperties", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Get properties for archived database.")]
        ArchiveProperties GetArchiveProperties(ArchivePropertiesRequest archive);

        [OperationContract(AsyncPattern = false)]
        [WebInvoke(Method = "POST", UriTemplate = "/UpdateArchiveProperties", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Updates archive properties.")]
        void UpdateArchiveSettings(ArchiveUpdateRequest request);

        [OperationContract(AsyncPattern = false)]
        [WebInvoke(Method = "POST", UriTemplate = "/DetachArchive", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Detach archive with events from SQL Compliance Manager.")]
        void DetachArchive(DetachArchiveRequest request);

        [OperationContract(AsyncPattern = false)]
        [WebInvoke(Method = "POST", UriTemplate = "/CheckNeedIndexUpdatesForArchive", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Check if index update for archive database is needed.")]
        bool CheckNeedIndexUpdates(ArchivePropertiesRequest archive);

        [OperationContract(AsyncPattern = false)]
        [WebInvoke(Method = "GET", UriTemplate = "/IsIndexStartTimeForArchiveDatabaseIsValid", RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        [Description("Check if indexing schedule is valid for archive database.")]
        bool IsIndexStartTimeIsValid();

        [OperationContract(AsyncPattern = false)]
        [WebInvoke(Method = "POST", UriTemplate = "/ApplyReindexForArchive", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Apply re-index for archive database.")]
        void ApplyReindex(DatabaseReindexRequest request);

        [OperationContract(AsyncPattern = false)]
        [WebInvoke(Method = "POST", UriTemplate = "/AttachArchive", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Attach archive for SQL Compliance.")]
        void AttachArchive(string archive);
    }
}
