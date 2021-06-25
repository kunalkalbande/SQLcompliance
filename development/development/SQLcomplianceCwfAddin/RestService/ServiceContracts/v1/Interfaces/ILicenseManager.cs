using System;
using System.ComponentModel;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SQLcomplianceCwfAddin.RestService.DataContracts.v1.License;

namespace SQLcomplianceCwfAddin.RestService.ServiceContracts.v1.Interfaces
{
    [ServiceContract]
    public interface ILicenseManager
    {
        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "License/GetLicense", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Get details of the current license.")]
        LicenseDetails GetLicense();
    }
}
