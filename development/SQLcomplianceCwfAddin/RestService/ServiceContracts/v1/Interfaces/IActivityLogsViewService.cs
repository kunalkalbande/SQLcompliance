using System.Collections.Generic;
using System.ComponentModel;
using System.ServiceModel;
using System.ServiceModel.Web;
using SQLcomplianceCwfAddin.RestService.DataContracts.v1;
using SQLcomplianceCwfAddin.RestService.DataContracts.v1.ActivityLogs;

namespace SQLcomplianceCwfAddin.RestService.ServiceContracts.v1.Interfaces
{
    [ServiceContract]
   public interface IActivityLogsViewService
    {
        [OperationContract(AsyncPattern = false)]
        [WebInvoke(Method = "POST", UriTemplate = "/GetFilteredActivityLogs", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Get alerts based on filter.")]
        FilteredActivityLogsViewResponce GetFilteredActivityLogs(FilteredActivityLogsViewRequest request);

        [OperationContract(AsyncPattern = false)]
        [WebInvoke(Method = "POST", UriTemplate = "/GetActivityProperties", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Get SQL Compliance Manager agent alert event properties.")]
        ServerActivityLogs GetActivityProperties(FilteredActivityLogsViewRequest request);
   }
}
