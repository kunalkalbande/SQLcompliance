using System.Collections.Generic;
using System.ComponentModel;
using System.ServiceModel;
using System.ServiceModel.Web;
using SQLcomplianceCwfAddin.RestService.DataContracts.v1;
using SQLcomplianceCwfAddin.RestService.DataContracts.v1.UserSettings;

namespace SQLcomplianceCwfAddin.RestService.ServiceContracts.v1.Interfaces
{
    [ServiceContract]
    public interface IUserSettingsService
    {
        [OperationContract(AsyncPattern = false)]
        [WebInvoke(Method = "GET", UriTemplate = "/GetUserSettings?dashboardUserId={dashboardUserId}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Get user settings by id.")]
        UserSettingsModel GetUserSettings(int dashboardUserId);

        [OperationContract(AsyncPattern = false)]
        [WebInvoke(Method = "GET", UriTemplate = "/GetAllUserSettings", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Get all user settings.")]
        IEnumerable<UserSettingsModel> GetAllUserSettings();

        [OperationContract(AsyncPattern = false)]
        [WebInvoke(Method = "POST", UriTemplate = "/CreateUpdateUserSettings", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Create or update user settings.")]
        void CreateUpdateUserSettings(UserSettingsModel userSettings);

        [OperationContract(AsyncPattern = false)]
        [WebInvoke(Method = "POST", UriTemplate = "/DeleteUserSettings", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Delete user settings.")]
        void DeleteUserSettings(DeleteUserSettingsRequest deleteUserSettingsRequest);

        [OperationContract(AsyncPattern = false)]
        [WebInvoke(Method = "POST", UriTemplate = "/SetRefreshDuration", RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        [Description("Set Auto Refresh Duration.")]
        string SetRefreshDuration(string refreshDuration);

        [OperationContract(AsyncPattern = false)]
        [WebInvoke(Method = "POST", UriTemplate = "/GetRefreshDuration", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Get Auto Refresh Duration.")]
        string GetRefreshDuration();
    }
}
