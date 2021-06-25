using System.Collections.Generic;
using System.ComponentModel;
using System.ServiceModel;
using System.ServiceModel.Web;
using SQLcomplianceCwfAddin.RestService.DataContracts.v1;
using SQLcomplianceCwfAddin.RestService.DataContracts.v1.ChangeLogs;

namespace SQLcomplianceCwfAddin.RestService.ServiceContracts.v1.Interfaces
{
    [ServiceContract]
    public interface IChangeLogsViewService
    {
        [OperationContract(AsyncPattern = false)]
        [WebInvoke(Method = "POST", UriTemplate = "/GetFilteredChangeLogs", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Get alerts based on filter.")]
        FilteredChangeLogsViewResponce GetFilteredChangeLogs(FilteredChangeLogsViewRequest request);

        [OperationContract(AsyncPattern = false)]
        [WebInvoke(Method = "POST", UriTemplate = "/GetChangeProperties", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Get SQL Compliance Manager agent alert event properties.")]
        ServerChangeLogs GetChangeProperties(FilteredChangeLogsViewRequest request);
    }
}
