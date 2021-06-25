using System.ComponentModel;
using System.ServiceModel;
using System.ServiceModel.Web;
using SQLcomplianceCwfAddin.RestService.DataContracts.v1;

namespace SQLcomplianceCwfAddin.RestService.ServiceContracts.v1.Interfaces
{
    [ServiceContract]
    public interface ILicenseService
    {
        [OperationContract(AsyncPattern = false)]
        [WebInvoke(Method = "GET", UriTemplate = "/CanAddOneMoreInstance", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Check is license server count lets to add a server instance.")]
        bool CanAddOneMoreInstance();

        [OperationContract(AsyncPattern = false)]
        [WebInvoke(Method = "GET", UriTemplate = "/GetCmLicenses", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Get SQL Compliance Manager license.")]
        CmCombinedLicense GetCmLicenses();

        [OperationContract(AsyncPattern = false)]
        [WebInvoke(Method = "POST", UriTemplate = "/AddLicense", RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        [Description("Add SQL Compliance Manager license.")]
        AddLicenseResponse AddLicense(string licenseKey);

        [OperationContract(AsyncPattern = false)]
        [WebInvoke(Method = "POST", UriTemplate = "/RemoveLicense", RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        [Description("Remove SQL Compliance Manager license.")]
        void RemoveLicense(int id);
    }
}
